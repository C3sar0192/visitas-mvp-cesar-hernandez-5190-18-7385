using Api.Datos;
using Api.Modelos;

namespace Api.Servicios
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ServicioReportes
    {
        private readonly AppDbContext _db;
        private readonly ServicioConfiguraciones _cfg;

        public ServicioReportes(AppDbContext db, ServicioConfiguraciones cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        public string RenderizarHtml(long idVisita, out string asunto)
        {
            Visita? v = _db.Visitas.Find(idVisita);
            if (v == null)
            {
                throw new InvalidOperationException($"No existe la visita {idVisita}.");
            }

            Cliente? cli = _db.Clientes.Find(v.IdCliente);
            Usuario? tec = _db.Usuarios.Find(v.IdTecnico);
            Usuario? sup = _db.Usuarios.Find(v.IdSupervisor);

            // -----------------------------------------
            // Eventos de la visita (CheckIn / CheckOut / Cancelación)
            // -----------------------------------------
            List<EventoVisita> eventos = _db.EventosVisita
                .Where(e => e.IdVisita == idVisita)
                .OrderBy(e => e.CreadoEn)
                .ToList();

            // Inicio: primer CheckIn (si existe)
            DateTime? inicio = eventos
                .Where(e => e.Tipo == TipoEventoVisita.CheckIn)
                .OrderBy(e => e.CreadoEn)
                .Select(e => (DateTime?)e.CreadoEn)
                .FirstOrDefault();

            // Fin: último CheckOut o Cancelación
            DateTime? fin = eventos
                .Where(e => e.Tipo == TipoEventoVisita.CheckOut ||
                            e.Tipo == TipoEventoVisita.Cancelacion)
                .OrderByDescending(e => e.CreadoEn)
                .Select(e => (DateTime?)e.CreadoEn)
                .FirstOrDefault();

            // Texto de fechas (si la fecha es muy vieja, la consideramos inválida)
            string inicioTxt = (inicio.HasValue && inicio.Value.Year > 1900)
                ? inicio.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                : "-";

            string finTxt = (fin.HasValue && fin.Value.Year > 1900)
                ? fin.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                : "-";

            // Duración en texto
            string duracionTexto;
            if (v.DuracionMinutos.HasValue && v.DuracionMinutos.Value > 0)
            {
                duracionTexto = $"{v.DuracionMinutos.Value} min";
            }
            else if (inicio.HasValue && fin.HasValue && fin > inicio && fin.Value.Year > 1900)
            {
                int minutos = (int)Math.Round((fin.Value - inicio.Value).TotalMinutes);
                duracionTexto = $"{minutos} min";
            }
            else
            {
                duracionTexto = "-";
            }

            // Última nota no vacía (normalmente del CheckOut o Cancelación)
            string notas = eventos
                .Where(e => !string.IsNullOrWhiteSpace(e.Notas))
                .OrderByDescending(e => e.CreadoEn)
                .Select(e => e.Notas)
                .FirstOrDefault() ?? "-";

            // Plantilla base
            string plantilla = """
                <h3>Reporte visita #{{Id}}</h3>
                <p><strong>Cliente:</strong> {{Cliente}}</p>
                <p><strong>Técnico:</strong> {{Tecnico}}</p>
                <p><strong>Supervisor:</strong> {{Supervisor}}</p>
                <p><strong>Inicio:</strong> {{Inicio}}</p>
                <p><strong>Fin:</strong> {{Fin}}</p>
                <p><strong>Duración:</strong> {{Duracion}}</p>
                <p><strong>Notas:</strong> {{Notas}}</p>
                <p><strong>Mapa:</strong> <a href="{{Mapa}}">{{Mapa}}</a></p>
                """;

            string html = plantilla
                .Replace("{{Id}}", v.Id.ToString())
                .Replace("{{Cliente}}", cli?.Nombre ?? "")
                .Replace("{{Tecnico}}", tec?.Nombre ?? "")
                .Replace("{{Supervisor}}", sup?.Nombre ?? "")
                .Replace("{{Inicio}}", inicioTxt)
                .Replace("{{Fin}}", finTxt)
                .Replace("{{Duracion}}", duracionTexto)
                .Replace("{{Notas}}", notas)
                .Replace("{{Mapa}}", cli?.UrlMapa ?? "");

            asunto = $"Reporte de visita #{v.Id}";
            return html;
        }

        public ReporteVisita GuardarHtml(long idVisita, string html)
        {
            ReporteVisita r = new ReporteVisita
            {
                IdVisita = idVisita,
                Formato = "HTML",
                ContenidoHtml = html,
                GeneradoEn = DateTime.UtcNow,
                GeneradoPor = "sistema"
            };
            _db.ReportesVisita.Add(r);
            _db.SaveChanges();
            return r;
        }
    }
}
