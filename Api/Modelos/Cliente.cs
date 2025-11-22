namespace Api.Modelos
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class Cliente
    {
        /// <summary>
        /// Identificacion del cliente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Dección del cliente
        /// </summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Latitud para la ubicación del cliente
        /// </summary>
        public decimal Latitud { get; set; } // opcional llenar desde UrlMapa

        /// <summary>
        /// Longitud que sirve par la ubicación del cliente
        /// </summary>
        public decimal Longitud { get; set; }

        /// <summary>
        /// Correo del cliente para el contacto
        /// </summary>
        public string? CorreoContacto { get; set; }

        /// <summary>
        /// Telefono de contacto del cliente
        /// </summary>
        public string? TelefonoContacto { get; set; }

        /// <summary>
        /// Url del mapa del cliente
        /// </summary>
        public string? UrlMapa { get; set; } // https://maps.app.goo.gl/...
    }
}
