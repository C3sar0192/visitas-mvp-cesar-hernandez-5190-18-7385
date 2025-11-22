namespace Api.Modelos.dto
{

    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A

    /// <summary>
    /// DTO genérico para devolver información de usuarios
    /// hacia el cliente Web.
    /// </summary>
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? Correo { get; set; }

        /// <summary>
        /// Identificador del supervisor al que está asignado
        /// el usuario (solo aplica para técnicos).
        /// </summary>
        public int? IdSupervisor { get; set; }

        /// <summary>
        /// Nombre del supervisor (si se cargó la navegación).
        /// </summary>
        public string? NombreSupervisor { get; set; }
    }
}
