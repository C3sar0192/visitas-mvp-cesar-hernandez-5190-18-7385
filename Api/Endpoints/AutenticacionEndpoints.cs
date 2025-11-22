using Api.Datos;
using Api.Modelos;
using Api.Modelos.dto;
using Api.Servicios;

namespace Api.Endpoints
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public static class AutenticacionEndpoints
    {
        public static void Mapear(WebApplication app)
        {
            app.MapPost("/autenticacion/iniciar-sesion",
                (PeticionLogin p, AppDbContext db, ServicioJwt jwt) =>
                {
                    // IMPORTANTE: solo proyectamos las columnas necesarias
                    var dato = db.Usuarios
                        .Where(u => u.Correo == p.Correo && u.Contrasena == p.Contrasena)
                        .Select(u => new
                        {
                            u.Id,
                            u.Nombre,
                            u.Correo,
                            u.Rol
                        })
                        .SingleOrDefault();

                    if (dato == null)
                    {
                        // Credenciales inválidas
                        return Results.Unauthorized();
                    }

                    // Creamos un Usuario "ligero" solo con los datos necesarios
                    Usuario uLite = new Usuario
                    {
                        Id = dato.Id,
                        Nombre = dato.Nombre,
                        Correo = dato.Correo,
                        Rol = dato.Rol
                        // IdSupervisor, etc. se quedan con su valor por defecto,
                        // y no se usan para emitir el token.
                    };

                    string token = jwt.EmitirToken(uLite);

                    RespuestaLogin r = new RespuestaLogin
                    {
                        Token = token,
                        Rol = uLite.Rol,
                        Nombre = uLite.Nombre
                    };

                    return Results.Ok(r);
                });
        }
    }
}
