namespace Api.Modelos
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ReporteVisita
    {
        public long Id { get; set; }
        public long IdVisita { get; set; }
        public string Formato { get; set; } = "HTML"; // HTML|PDF
        public string? RutaArchivo { get; set; }
        public string? ContenidoHtml { get; set; }
        public DateTime GeneradoEn { get; set; } = DateTime.UtcNow;
        public string GeneradoPor { get; set; } = "sistema";
    }
}
