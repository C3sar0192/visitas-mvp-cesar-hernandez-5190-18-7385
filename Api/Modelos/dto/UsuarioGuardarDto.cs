namespace Api.Modelos.dto
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A

    /// <summary>
    /// DTO usado para crear o editar usuarios desde el módulo
    /// de mantenimiento (nombre, correo, rol, contraseña y
    /// supervisor opcional).
    /// </summary>
    public class UsuarioGuardarDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña en texto claro. En un sistema real debería
        /// guardarse con hash, pero para el MVP se compara tal cual.
        /// </summary>
        public string Contrasena { get; set; } = string.Empty;

        /// <summary>
        /// Supervisor al que se asigna el técnico (solo aplica
        /// cuando Rol = "Tecnico").
        /// </summary>
        public int? IdSupervisor { get; set; }
    }
}
