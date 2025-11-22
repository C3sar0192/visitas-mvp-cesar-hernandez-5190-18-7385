using System;
using Api.Datos;
using Api.Modelos;
using Api.Modelos.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public static class ReportesEndpoints
    {
        public static void Mapear(WebApplication app)
        {
            // ----------------------------------------------------
            // 1) Ver reporte de una visita (HTML o PDF)
            //    GET /reportes/visita/{id}
            // ----------------------------------------------------
            app.MapGet("/reportes/visita/{id:long}", [Authorize(Roles = "Administrador,Supervisor")] (long id, AppDbContext db) =>
            {
                ReporteVisita? r = db.ReportesVisita
                    .Where(x => x.IdVisita == id)
                    .OrderByDescending(x => x.GeneradoEn)
                    .FirstOrDefault();

                if (r == null) return Results.NotFound();

                if (r.Formato == "HTML" && !string.IsNullOrWhiteSpace(r.ContenidoHtml))
                {
                    return Results.Content(r.ContenidoHtml!, "text/html");
                }

                if (!string.IsNullOrWhiteSpace(r.RutaArchivo) && System.IO.File.Exists(r.RutaArchivo))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(r.RutaArchivo!);
                    return Results.File(bytes, "application/pdf", $"reporte_visita_{id}.pdf");
                }

                return Results.NotFound();
            });

            // ----------------------------------------------------
            // 2) Reporte de visitas por técnico (JSON)
            //
            //    GET /reportes/visitas-por-tecnico?tecnicoId=3&desde=2025-01-01&hasta=2025-01-31
            //
            //    - tecnicoId: Id del técnico (obligatorio)
            //    - desde / hasta: rango de fechas opcional (UTC)
            //      si no vienen, se toma último mes por defecto.
            //
            //    Solo para Administrador y Supervisor.
            // ----------------------------------------------------
            app.MapGet("/reportes/visitas-por-tecnico",
                [Authorize(Roles = "Administrador,Supervisor")] async (
                    int tecnicoId,
                    DateTime? desde,
                    DateTime? hasta,
                    AppDbContext db) =>
                {
                    DateTime inicio = (desde ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                    DateTime fin = (hasta ?? DateTime.UtcNow.Date).Date.AddDays(1); // exclusivo

                    IQueryable<Visita> q = db.Visitas
                        .Where(v => v.IdTecnico == tecnicoId &&
                                    v.FechaProgramada >= inicio &&
                                    v.FechaProgramada < fin);

                    List<Visita> visitas = await q
                        .OrderBy(v => v.FechaProgramada)
                        .ToListAsync();

                    List<VisitaReporteDto> resultado = new List<VisitaReporteDto>();

                    foreach (Visita v in visitas)
                    {
                        Cliente? c = db.Clientes.Find(v.IdCliente);
                        Usuario? t = db.Usuarios.Find(v.IdTecnico);
                        Usuario? s = db.Usuarios.Find(v.IdSupervisor);

                        VisitaReporteDto dto = new VisitaReporteDto
                        {
                            IdVisita = v.Id,
                            NombreCliente = c?.Nombre ?? "",
                            NombreTecnico = t?.Nombre ?? "",
                            NombreSupervisor = s?.Nombre ?? "",
                            FechaProgramada = v.FechaProgramada,
                            Estado = v.Estado,
                            DuracionMinutos = v.DuracionMinutos
                        };

                        resultado.Add(dto);
                    }

                    return Results.Ok(resultado);
                });
        }
    }
}
