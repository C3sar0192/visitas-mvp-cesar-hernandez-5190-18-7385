namespace Api.Modelos
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class EventoVisita
    {
        public long Id { get; set; }
        public long IdVisita { get; set; }
        public TipoEventoVisita Tipo { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string? Notas { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
