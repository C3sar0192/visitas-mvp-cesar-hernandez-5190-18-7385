using System;

namespace Api.Modelos.dto
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class VisitaDto
    {
        public long Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreSupervisor { get; set; } = string.Empty;
        public string NombreTecnico { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public EstadoVisita Estado { get; set; }
        public string? UrlMapaCliente { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        // NOTA de planificación (Visita.Notas)
        public string? NotasVisita { get; set; }

        // NOTA del técnico (último evento CheckOut/Cancelación)
        public string? NotasTecnico { get; set; }
    }

    public class CrearVisitaDto
    {
        public int IdCliente { get; set; }
        public int IdSupervisor { get; set; }
        public int IdTecnico { get; set; }

        // el Web envía algo tipo "2025-11-20T10:30"
        public string FechaProgramada { get; set; } = string.Empty;

        // NOTA de planificación (la escribe el que crea la visita)
        public string? Notas { get; set; }
    }

    public class EventoVisitaDto
    {
        public TipoEventoVisita Tipo { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string? Notas { get; set; }
    }
}
