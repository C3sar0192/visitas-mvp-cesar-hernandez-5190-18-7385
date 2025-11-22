namespace Api.Modelos
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ConfiguracionSistema
    {
        public string Clave { get; set; } = string.Empty;   // PK
        public string Valor { get; set; } = string.Empty;
        public string Tipo { get; set; } = "string";        // bool|int|string|text|json
        public string ActualizadoPor { get; set; } = "sistema";
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;
    }
}
