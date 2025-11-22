using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Modelos;
using Microsoft.IdentityModel.Tokens;

namespace Api.Servicios
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ServicioJwt
    {
        private readonly IConfiguration _cfg;
        public ServicioJwt(IConfiguration cfg) { _cfg = cfg; }

        public string EmitirToken(Usuario u)
        {
            string emisor = _cfg["Jwt:Emisor"] ?? Environment.GetEnvironmentVariable("Jwt__Emisor") ?? "VisitasEmisor";
            string audiencia = _cfg["Jwt:Audiencia"] ?? Environment.GetEnvironmentVariable("Jwt__Audiencia") ?? "VisitasAudiencia";
            string clave = _cfg["Jwt:Clave"] ?? Environment.GetEnvironmentVariable("Jwt__Clave") ?? "CLAVE_DEMO_SUPER_SECRETA_CAMBIAR_32+_CARACTERES";

            byte[] bytes = Encoding.UTF8.GetBytes(clave);
            SymmetricSecurityKey key = new SymmetricSecurityKey(bytes);
            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
                new Claim(ClaimTypes.Name, u.Nombre),
                new Claim(ClaimTypes.Email, u.Correo),
                new Claim(ClaimTypes.Role, u.Rol)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: emisor,
                audience: audiencia,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
