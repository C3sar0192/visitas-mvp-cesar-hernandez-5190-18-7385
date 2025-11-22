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
    public static class ClientesEndpoints
    {
        public static void Mapear(WebApplication app)
        {
            RouteGroupBuilder g = app.MapGroup("/clientes").RequireAuthorization();

            g.MapGet("", [Authorize(Roles = "Administrador,Supervisor")] async (AppDbContext db) =>
            {
                List<ClienteDto> lista = await db.Clientes
                    .Select(c => new ClienteDto
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Direccion = c.Direccion,
                        CorreoContacto = c.CorreoContacto,
                        TelefonoContacto = c.TelefonoContacto,
                        UrlMapa = c.UrlMapa
                    }).ToListAsync();
                return Results.Ok(lista);
            });

            g.MapGet("/{id:int}", [Authorize(Roles = "Administrador,Supervisor")] async (int id, AppDbContext db) =>
            {
                Cliente? c = await db.Clientes.FindAsync(id);
                if (c == null) return Results.NotFound();
                ClienteDto d = new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Direccion = c.Direccion,
                    CorreoContacto = c.CorreoContacto,
                    TelefonoContacto = c.TelefonoContacto,
                    UrlMapa = c.UrlMapa
                };
                return Results.Ok(d);
            });

            g.MapPost("", [Authorize(Roles = "Administrador,Supervisor")] async (CrearClienteDto m, AppDbContext db) =>
            {
                Cliente c = new Cliente
                {
                    Nombre = m.Nombre,
                    Direccion = m.Direccion,
                    CorreoContacto = m.CorreoContacto,
                    TelefonoContacto = m.TelefonoContacto,
                    UrlMapa = m.UrlMapa ?? ""
                };

                // Intentar extraer lat/lon desde UrlMapa (MVP)
                if (!string.IsNullOrWhiteSpace(c.UrlMapa))
                {
                    (decimal? lat, decimal? lon) = ExtraerCoordenadas(c.UrlMapa);
                    if (lat.HasValue && lon.HasValue) { c.Latitud = lat.Value; c.Longitud = lon.Value; }
                }

                db.Clientes.Add(c);
                await db.SaveChangesAsync();
                return Results.Created($"/clientes/{c.Id}", new { c.Id });
            });

            g.MapPut("/{id:int}", [Authorize(Roles = "Administrador,Supervisor")] async (int id, CrearClienteDto m, AppDbContext db) =>
            {
                Cliente? c = await db.Clientes.FindAsync(id);
                if (c == null) return Results.NotFound();

                c.Nombre = m.Nombre; c.Direccion = m.Direccion;
                c.CorreoContacto = m.CorreoContacto; c.TelefonoContacto = m.TelefonoContacto;
                c.UrlMapa = m.UrlMapa ?? "";

                if (!string.IsNullOrWhiteSpace(c.UrlMapa))
                {
                    (decimal? lat, decimal? lon) = ExtraerCoordenadas(c.UrlMapa);
                    if (lat.HasValue && lon.HasValue) { c.Latitud = lat.Value; c.Longitud = lon.Value; }
                }

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            g.MapDelete("/{id:int}", [Authorize(Roles = "Administrador")] async (int id, AppDbContext db) =>
            {
                Cliente? c = await db.Clientes.FindAsync(id);
                if (c == null) return Results.NotFound();
                db.Clientes.Remove(c);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }

        private static (decimal?, decimal?) ExtraerCoordenadas(string url)
        {
            System.Text.RegularExpressions.Match m;

            m = System.Text.RegularExpressions.Regex.Match(url, @"@(-?\d+(?:\.\d+)?),\s*(-?\d+(?:\.\d+)?)");
            if (m.Success) return (decimal.Parse(m.Groups[1].Value), decimal.Parse(m.Groups[2].Value));

            m = System.Text.RegularExpressions.Regex.Match(url, @"[?&]q=(-?\d+(?:\.\d+)?),\s*(-?\d+(?:\.\d+)?)");
            if (m.Success) return (decimal.Parse(m.Groups[1].Value), decimal.Parse(m.Groups[2].Value));

            m = System.Text.RegularExpressions.Regex.Match(url, @"!3d(-?\d+(?:\.\d+)?)!4d(-?\d+(?:\.\d+)?)");
            if (m.Success) return (decimal.Parse(m.Groups[1].Value), decimal.Parse(m.Groups[2].Value));

            m = System.Text.RegularExpressions.Regex.Match(url, @"(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)");
            if (m.Success) return (decimal.Parse(m.Groups[1].Value), decimal.Parse(m.Groups[2].Value));

            return (null, null);
        }
    }
}
