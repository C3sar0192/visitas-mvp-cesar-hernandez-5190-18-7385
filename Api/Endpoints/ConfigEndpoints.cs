using System.Security.Claims;
using Api.Datos;
using Api.Modelos.dto;
using Api.Servicios;
using Microsoft.AspNetCore.Authorization;

namespace Api.Endpoints
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public static class ConfigEndpoints
    {
        public static void Mapear(WebApplication app)
        {
            RouteGroupBuilder g = app.MapGroup("/config").RequireAuthorization();

            g.MapGet("", [Authorize(Roles = "Administrador,Supervisor")] (AppDbContext db) =>
            {
                var lista = db.Configuraciones.Select(c => new ConfiguracionDto { Clave = c.Clave, Valor = c.Valor, Tipo = c.Tipo }).ToList();
                return Results.Ok(lista);
            });

            g.MapGet("/{clave}", [Authorize(Roles = "Administrador,Supervisor")] (string clave, AppDbContext db) =>
            {
                var c = db.Configuraciones.Find(clave);
                if (c == null) return Results.NotFound();
                return Results.Ok(new ConfiguracionDto { Clave = c.Clave, Valor = c.Valor, Tipo = c.Tipo });
            });

            g.MapPut("/{clave}", [Authorize(Roles = "Administrador,Supervisor")] (string clave, ConfiguracionDto d, ClaimsPrincipal user, ServicioConfiguraciones cfg) =>
            {
                string quien = user.Identity?.Name ?? "usuario";
                cfg.Guardar(clave, d.Valor, d.Tipo, quien);
                return Results.NoContent();
            });
        }
    }
}
