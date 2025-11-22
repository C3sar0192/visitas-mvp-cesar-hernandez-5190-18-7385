using System;

namespace Api.Modelos.dto
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class VisitaReporteDto
    {
        public long IdVisita { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreTecnico { get; set; } = string.Empty;
        public string NombreSupervisor { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public EstadoVisita Estado { get; set; }
        public int? DuracionMinutos { get; set; }
    }
}
