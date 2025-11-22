using System.Security.Claims;
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
    public static class UsuariosEndpoints
    {
        public static void Mapear(WebApplication app)
        {
            // ----------------------------------------------------
            // 1) Listar usuarios
            //    GET /usuarios
            // ----------------------------------------------------
            app.MapGet("/usuarios", [Authorize] async (string? rol, AppDbContext db) =>
            {
                IQueryable<Usuario> baseQuery = db.Usuarios;

                // Si quisieras filtrar por rol, podrías activarlo aquí
                if (!string.IsNullOrWhiteSpace(rol))
                {
                    string rolNormalizado = rol.Trim();
                    baseQuery = baseQuery.Where(u => u.Rol == rolNormalizado);
                }

                var lista = await baseQuery
                    .GroupJoin(
                        db.Usuarios,                 // “tabla” de supervisores
                        u => u.IdSupervisor,         // FK del técnico
                        s => s.Id,                   // PK del supervisor
                        (u, sups) => new { u, s = sups.FirstOrDefault() }
                    )
                    .OrderBy(x => x.u.Id)
                    .Select(x => new UsuarioDto
                    {
                        Id = x.u.Id,
                        Nombre = x.u.Nombre,
                        Correo = x.u.Correo,
                        Rol = x.u.Rol,
                        IdSupervisor = x.u.IdSupervisor,
                        NombreSupervisor = x.s != null ? x.s.Nombre : null
                    })
                    .ToListAsync();

                return Results.Ok(lista);
            });

            // ----------------------------------------------------
            // 2) Obtener usuario por Id
            //    GET /usuarios/1
            // ----------------------------------------------------
            app.MapGet("/usuarios/{id:int}", [Authorize] async (int id, AppDbContext db) =>
            {
                var dato = await db.Usuarios
                    .Where(u => u.Id == id)
                    .GroupJoin(
                        db.Usuarios,
                        u => u.IdSupervisor,
                        s => s.Id,
                        (u, sups) => new { u, s = sups.FirstOrDefault() }
                    )
                    .Select(x => new UsuarioDto
                    {
                        Id = x.u.Id,
                        Nombre = x.u.Nombre,
                        Correo = x.u.Correo,
                        Rol = x.u.Rol,
                        IdSupervisor = x.u.IdSupervisor,
                        NombreSupervisor = x.s != null ? x.s.Nombre : null
                    })
                    .FirstOrDefaultAsync();

                if (dato == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(dato);
            });

            // ----------------------------------------------------
            // 3) Crear usuario
            //    POST /usuarios
            // ----------------------------------------------------
            app.MapPost("/usuarios", [Authorize(Roles = "Administrador")] async (UsuarioGuardarDto m, AppDbContext db) =>
            {
                Usuario u = new Usuario
                {
                    Nombre = m.Nombre.Trim(),
                    Correo = m.Correo.Trim(),
                    Rol = m.Rol.Trim(),
                    Contrasena = m.Contrasena,
                    IdSupervisor = m.IdSupervisor
                };

                db.Usuarios.Add(u);
                await db.SaveChangesAsync();

                UsuarioDto dto = new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Rol = u.Rol,
                    IdSupervisor = u.IdSupervisor,
                    NombreSupervisor = null
                };

                return Results.Created($"/usuarios/{u.Id}", dto);
            });

            // ----------------------------------------------------
            // 4) Actualizar usuario
            //    PUT /usuarios/{id}
            // ----------------------------------------------------
            app.MapPut("/usuarios/{id:int}", [Authorize(Roles = "Administrador")] async (int id, UsuarioGuardarDto m, AppDbContext db) =>
            {
                Usuario? u = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

                if (u == null)
                {
                    return Results.NotFound();
                }

                u.Nombre = m.Nombre.Trim();
                u.Correo = m.Correo.Trim();
                u.Rol = m.Rol.Trim();
                u.IdSupervisor = m.IdSupervisor;

                if (!string.IsNullOrWhiteSpace(m.Contrasena))
                {
                    u.Contrasena = m.Contrasena;
                }

                await db.SaveChangesAsync();

                UsuarioDto dto = new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    Rol = u.Rol,
                    IdSupervisor = u.IdSupervisor,
                    NombreSupervisor = null
                };

                return Results.Ok(dto);
            });

            // ----------------------------------------------------
            // 5) Eliminar usuario
            //    DELETE /usuarios/{id}
            // ----------------------------------------------------
            app.MapDelete("/usuarios/{id:int}", [Authorize(Roles = "Administrador")] async (int id, AppDbContext db) =>
            {
                Usuario? u = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

                if (u == null)
                {
                    return Results.NotFound();
                }

                db.Usuarios.Remove(u);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // ----------------------------------------------------
            // 6) Técnicos a cargo de un supervisor (simple)
            //    GET /usuarios/mis-tecnicos
            // ----------------------------------------------------
            app.MapGet("/usuarios/mis-tecnicos",
                [Authorize(Roles = "Supervisor")] async (ClaimsPrincipal user, AppDbContext db) =>
                {
                    int idSupervisor = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                    List<Usuario> tecnicosAsignados = await db.Usuarios
                        .Where(u => u.Rol == "Tecnico" && u.IdSupervisor == idSupervisor)
                        .ToListAsync();

                    var visitasPorTecnico = await db.Visitas
                        .Where(v => v.IdSupervisor == idSupervisor)
                        .GroupBy(v => v.IdTecnico)
                        .Select(g => new
                        {
                            IdTecnico = g.Key,
                            Cantidad = g.Count()
                        })
                        .ToListAsync();

                    Dictionary<int, int> dicVisitas = visitasPorTecnico
                        .ToDictionary(x => x.IdTecnico, x => x.Cantidad);

                    List<TecnicoResumenDto> resultado = new List<TecnicoResumenDto>();

                    foreach (Usuario t in tecnicosAsignados)
                    {
                        dicVisitas.TryGetValue(t.Id, out int cant);

                        TecnicoResumenDto dto = new TecnicoResumenDto
                        {
                            Id = t.Id,
                            Nombre = t.Nombre,
                            Correo = t.Correo,
                            CantidadVisitas = cant,
                            VisitasHoy = cant,
                            CantidadVisitasHoy = cant
                        };

                        resultado.Add(dto);
                    }

                    return Results.Ok(resultado);
                });

            // ----------------------------------------------------
            // 7) Resumen de carga de trabajo por técnico (Supervisor)
            //    GET /usuarios/mis-tecnicos-resumen?fecha=2025-11-30
            // ----------------------------------------------------
            app.MapGet("/usuarios/mis-tecnicos-resumen",
                [Authorize(Roles = "Supervisor")] async (ClaimsPrincipal user, DateTime? fecha, AppDbContext db) =>
                {
                    int idSupervisor = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    DateTime dia = (fecha ?? DateTime.UtcNow).Date;
                    DateTime diaSiguiente = dia.AddDays(1);

                    List<Usuario> tecnicosAsignados = await db.Usuarios
                        .Where(u => u.Rol == "Tecnico" && u.IdSupervisor == idSupervisor)
                        .ToListAsync();

                    var visitasAgrupadas = await db.Visitas
                        .Where(v => v.IdSupervisor == idSupervisor)
                        .GroupBy(v => v.IdTecnico)
                        .Select(g => new
                        {
                            IdTecnico = g.Key,
                            CantidadTotal = g.Count()
                        })
                        .ToListAsync();

                    var visitasHoyAgrupadas = await db.Visitas
                        .Where(v => v.IdSupervisor == idSupervisor &&
                                    v.FechaProgramada >= dia &&
                                    v.FechaProgramada < diaSiguiente)
                        .GroupBy(v => v.IdTecnico)
                        .Select(g => new
                        {
                            IdTecnico = g.Key,
                            Cantidad = g.Count()
                        })
                        .ToListAsync();

                    Dictionary<int, int> dicTotal = visitasAgrupadas
                        .ToDictionary(x => x.IdTecnico, x => x.CantidadTotal);

                    Dictionary<int, int> dicHoy = visitasHoyAgrupadas
                        .ToDictionary(x => x.IdTecnico, x => x.Cantidad);

                    List<TecnicoResumenDto> lista = new List<TecnicoResumenDto>();

                    foreach (Usuario t in tecnicosAsignados)
                    {
                        dicTotal.TryGetValue(t.Id, out int cantTotal);
                        dicHoy.TryGetValue(t.Id, out int cantHoy);

                        TecnicoResumenDto dto = new TecnicoResumenDto
                        {
                            Id = t.Id,
                            Nombre = t.Nombre,
                            Correo = t.Correo,
                            CantidadVisitas = cantTotal,
                            VisitasHoy = cantHoy,
                            CantidadVisitasHoy = cantHoy
                        };

                        lista.Add(dto);
                    }

                    return Results.Ok(lista);
                });
        }
    }
}
