namespace Api.Modelos.dto
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? CorreoContacto { get; set; }
        public string? TelefonoContacto { get; set; }
        public string? UrlMapa { get; set; }
    }

    public class CrearClienteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? CorreoContacto { get; set; }
        public string? TelefonoContacto { get; set; }
        public string? UrlMapa { get; set; }
    }
}
