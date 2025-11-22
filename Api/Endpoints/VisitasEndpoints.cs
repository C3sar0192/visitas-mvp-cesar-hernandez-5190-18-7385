using System.Security.Claims;
using Api.Datos;
using Api.Modelos;
using Api.Modelos.dto;
using Api.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public static class VisitasEndpoints
    {
        public static void Mapear(WebApplication app)
        {
            RouteGroupBuilder g = app.MapGroup("/visitas").RequireAuthorization();

            // ==========================================================
            // CREAR VISITA  (Admin / Supervisor)
            // ==========================================================
            g.MapPost("",
                [Authorize(Roles = "Administrador,Supervisor")]
            async (ClaimsPrincipal user, CrearVisitaDto m, AppDbContext db) =>
            {
                string rol = user.FindFirstValue(ClaimTypes.Role)!;
                int idLogueado = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                int idSupervisorReal = rol == "Supervisor"
                    ? idLogueado
                    : m.IdSupervisor;

                DateTime fecha;
                if (!DateTime.TryParse(m.FechaProgramada, out fecha))
                {
                    fecha = DateTime.Now;
                }

                Visita v = new Visita
                {
                    IdCliente = m.IdCliente,
                    IdSupervisor = idSupervisorReal,
                    IdTecnico = m.IdTecnico,
                    FechaProgramada = fecha,
                    Estado = EstadoVisita.Programada,
                    Notas = m.Notas               // ← nota de planificación
                };

                db.Visitas.Add(v);
                await db.SaveChangesAsync();

                return Results.Created($"/visitas/{v.Id}", new { v.Id });
            });

            // ==========================================================
            // TABLERO (Admin ve todo; Supervisor solo su equipo)
            // ==========================================================
            g.MapGet("",
                [Authorize(Roles = "Administrador,Supervisor")]
            async (ClaimsPrincipal user, AppDbContext db) =>
            {
                string? rol = user.FindFirstValue(ClaimTypes.Role);
                int idUser = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                IQueryable<Visita> q = db.Visitas.AsQueryable();
                if (rol == "Supervisor")
                {
                    q = q.Where(x => x.IdSupervisor == idUser);
                }

                List<Visita> datos = await q
                    .OrderByDescending(x => x.FechaProgramada)
                    .Take(200)
                    .ToListAsync();

                List<VisitaDto> dto = new List<VisitaDto>();

                foreach (Visita v in datos)
                {
                    Cliente c = db.Clientes.Find(v.IdCliente)!;
                    Usuario s = db.Usuarios.Find(v.IdSupervisor)!;
                    Usuario t = db.Usuarios.Find(v.IdTecnico)!;

                    // última nota del técnico
                    string? notaTec = db.EventosVisita
                        .Where(e => e.IdVisita == v.Id &&
                               (e.Tipo == TipoEventoVisita.CheckOut ||
                                e.Tipo == TipoEventoVisita.Cancelacion))
                        .OrderBy(e => e.CreadoEn)
                        .Select(e => e.Notas)
                        .LastOrDefault();

                    dto.Add(new VisitaDto
                    {
                        Id = v.Id,
                        NombreCliente = c?.Nombre ?? "",
                        NombreSupervisor = s?.Nombre ?? "",
                        NombreTecnico = t?.Nombre ?? "",
                        FechaProgramada = v.FechaProgramada,
                        Estado = v.Estado,
                        UrlMapaCliente = c?.UrlMapa,
                        Latitud = c?.Latitud,
                        Longitud = c?.Longitud,
                        NotasVisita = v.Notas,
                        NotasTecnico = notaTec
                    });
                }

                return Results.Ok(dto);
            });

            // ==========================================================
            // VISITAS DEL TÉCNICO (listado "Hoy")
            // ==========================================================
            g.MapGet("/hoy",
                [Authorize(Roles = "Tecnico")]
            async (ClaimsPrincipal user, AppDbContext db) =>
            {
                int idTec = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                List<Visita> datos = await db.Visitas
                    .Where(v => v.IdTecnico == idTec)
                    .OrderBy(v => v.FechaProgramada)
                    .ToListAsync();

                List<VisitaDto> dto = new List<VisitaDto>();

                foreach (Visita v in datos)
                {
                    Cliente c = db.Clientes.Find(v.IdCliente)!;
                    Usuario t = db.Usuarios.Find(v.IdTecnico)!;
                    Usuario s = db.Usuarios.Find(v.IdSupervisor)!;

                    string? notaTec = db.EventosVisita
                        .Where(e => e.IdVisita == v.Id &&
                               (e.Tipo == TipoEventoVisita.CheckOut ||
                                e.Tipo == TipoEventoVisita.Cancelacion))
                        .OrderBy(e => e.CreadoEn)
                        .Select(e => e.Notas)
                        .LastOrDefault();

                    dto.Add(new VisitaDto
                    {
                        Id = v.Id,
                        NombreCliente = c?.Nombre ?? "",
                        NombreSupervisor = s?.Nombre ?? "",
                        NombreTecnico = t?.Nombre ?? "",
                        FechaProgramada = v.FechaProgramada,
                        Estado = v.Estado,
                        UrlMapaCliente = c?.UrlMapa,
                        Latitud = c?.Latitud,
                        Longitud = c?.Longitud,
                        NotasVisita = v.Notas,
                        NotasTecnico = notaTec
                    });
                }

                return Results.Ok(dto);
            });

            // ==========================================================
            // REGISTRAR EVENTO (CheckIn / CheckOut / Cancelación)
            // ==========================================================
            g.MapPost("/{id:long}/eventos",
                [Authorize(Roles = "Tecnico")]
            async (
                long id,
                EventoVisitaDto e,
                ClaimsPrincipal user,
                AppDbContext db,
                ServicioReportes rep,
                ServicioConfiguraciones cfg,
                ServicioEmail email) =>
            {
                Visita? v = await db.Visitas.FindAsync(id);
                if (v == null) return Results.NotFound();

                int idTec = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (v.IdTecnico != idTec) return Results.Forbid();

                EventoVisita ev = new EventoVisita
                {
                    IdVisita = id,
                    Tipo = e.Tipo,
                    Latitud = e.Latitud,
                    Longitud = e.Longitud,
                    Notas = e.Notas ?? string.Empty
                };
                db.EventosVisita.Add(ev);

                if (e.Tipo == TipoEventoVisita.CheckIn)
                {
                    v.Estado = EstadoVisita.EnCurso;
                }

                if (e.Tipo == TipoEventoVisita.CheckOut)
                {
                    v.Estado = EstadoVisita.Completada;

                    DateTime? inicio = db.EventosVisita
                        .Where(x => x.IdVisita == id && x.Tipo == TipoEventoVisita.CheckIn)
                        .OrderBy(x => x.CreadoEn)
                        .Select(x => (DateTime?)x.CreadoEn)
                        .FirstOrDefault();

                    if (inicio.HasValue)
                    {
                        v.DuracionMinutos = (int)Math.Round((DateTime.UtcNow - inicio.Value).TotalMinutes);
                    }
                }

                if (e.Tipo == TipoEventoVisita.Cancelacion)
                {
                    v.Estado = EstadoVisita.Cancelada;
                }

                // Primero guardamos eventos + cambios
                await db.SaveChangesAsync();

                // Solo en CheckOut se envía reporte por correo
                if (e.Tipo == TipoEventoVisita.CheckOut)
                {
                    bool enviar = cfg.ObtenerBool("Reportes.ActivarEnvioAlCerrar", true);
                    if (enviar)
                    {
                        string html = rep.RenderizarHtml(id, out string asunto);
                        ReporteVisita r = rep.GuardarHtml(id, html);

                        Cliente? c = db.Clientes.Find(v.IdCliente);
                        if (!string.IsNullOrWhiteSpace(c?.CorreoContacto))
                        {
                            string cuerpoIntro = cfg.ObtenerStr(
                                "Reportes.CuerpoEmail",
                                "Adjuntamos el reporte de su visita."
                            );

                            await email.EnviarAsync(
                                c.CorreoContacto!,
                                asunto,
                                cuerpoIntro + "<hr/>" + html
                            );
                        }
                    }
                }

                return Results.Ok(new { ok = true });
            });
        }
    }
}
