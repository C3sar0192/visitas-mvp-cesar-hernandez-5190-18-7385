using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Modelos
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class Visita
    {
        public long Id { get; set; }
        public int IdCliente { get; set; }
        public int IdSupervisor { get; set; }
        public int IdTecnico { get; set; }
        public DateTime FechaProgramada { get; set; }
        public EstadoVisita Estado { get; set; } = EstadoVisita.Programada;
        public int? DuracionMinutos { get; set; }

        // Nota de planificación (se guarda en la tabla Visitas)
        public string? Notas { get; set; }

        // Campos de apoyo no mapeados
        [NotMapped] public string? NombreCliente { get; set; }
        [NotMapped] public string? NombreSupervisor { get; set; }
        [NotMapped] public string? NombreTecnico { get; set; }
        [NotMapped] public string? UrlMapaCliente { get; set; }
        [NotMapped] public decimal? Latitud { get; set; }
        [NotMapped] public decimal? Longitud { get; set; }
    }
}
