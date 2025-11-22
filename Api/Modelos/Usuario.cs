namespace Api.Modelos
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class Usuario
    {
        public int Id { get; set; }

        /// <summary>
        /// Nombre visible del usuario (Admin, Super, Tec, etc.).
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico que también se utiliza como usuario
        /// para iniciar sesión.
        /// </summary>
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña en texto claro (solo para este MVP).
        /// </summary>
        public string Contrasena { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario: Administrador, Supervisor, Tecnico.
        /// </summary>
        public string Rol { get; set; } = string.Empty;

        /// <summary>
        /// Id del supervisor al que está asignado el técnico.
        /// Solo tiene valor cuando Rol = "Tecnico".
        /// </summary>
        public int? IdSupervisor { get; set; }
    }
}
