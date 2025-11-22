namespace Api.Modelos.dto
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class PeticionLogin
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }

    public class RespuestaLogin
    {
        public string Token { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
