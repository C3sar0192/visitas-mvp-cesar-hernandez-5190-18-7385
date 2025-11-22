namespace Api.Modelos.dto
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A

    /// <summary>
    /// DTO para mostrar los técnicos a cargo de un supervisor,
    /// junto con información de visitas.
    /// </summary>
    public class TecnicoResumenDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Correo { get; set; }

        /// <summary>
        /// Total de visitas históricas asignadas a este técnico
        /// por el supervisor actual.
        /// </summary>
        public int CantidadVisitas { get; set; }

        /// <summary>
        /// Visitas programadas para la fecha consultada.
        /// Se utiliza principalmente en el endpoint
        /// /usuarios/mis-tecnicos-resumen.
        /// </summary>
        public int VisitasHoy { get; set; }

        /// <summary>
        /// Campo auxiliar para el sitio WEB.
        /// En los endpoints del API siempre se llena con el mismo
        /// valor de VisitasHoy, para que el modelo
        /// Web.Models.TecnicoResumen.CantidadVisitasHoy
        /// reciba correctamente la información.
        /// </summary>
        public int CantidadVisitasHoy { get; set; }
    }
}
