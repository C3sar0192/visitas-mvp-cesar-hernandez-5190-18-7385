using System.Net;
using System.Net.Mail;

namespace Api.Servicios
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ServicioEmail
    {
        private readonly IConfiguration _cfg;
        public ServicioEmail(IConfiguration cfg) { _cfg = cfg; }

        public async Task EnviarAsync(string para, string asunto, string cuerpoHtml)
        {
            string host = _cfg["Smtp:Host"] ?? Environment.GetEnvironmentVariable("Smtp__Host") ?? "correo";
            int puerto = int.TryParse(_cfg["Smtp:Puerto"] ?? Environment.GetEnvironmentVariable("Smtp__Puerto"), out int p) ? p : 1025;
            string remitente = _cfg["Smtp:Remitente"] ?? Environment.GetEnvironmentVariable("Smtp__Remitente") ?? "noreply@demo.local";
            string? usuario = _cfg["Smtp:Usuario"] ?? Environment.GetEnvironmentVariable("Smtp__Usuario");
            string? pass = _cfg["Smtp:Contrasena"] ?? Environment.GetEnvironmentVariable("Smtp__Contrasena");

            using MailMessage mm = new MailMessage(remitente, para, asunto, cuerpoHtml);
            mm.IsBodyHtml = true;

            using SmtpClient cli = new SmtpClient(host, puerto);
            if (!string.IsNullOrEmpty(usuario))
            {
                cli.Credentials = new NetworkCredential(usuario, pass);
                cli.EnableSsl = true;
            }
            else
            {
                cli.UseDefaultCredentials = false; // Mailhog
            }
            await cli.SendMailAsync(mm);
        }
    }
}
