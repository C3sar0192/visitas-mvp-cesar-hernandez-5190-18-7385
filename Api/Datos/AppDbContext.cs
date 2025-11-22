using Api.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Api.Datos
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> o) : base(o) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Visita> Visitas => Set<Visita>();
        public DbSet<EventoVisita> EventosVisita => Set<EventoVisita>();
        public DbSet<ConfiguracionSistema> Configuraciones => Set<ConfiguracionSistema>();
        public DbSet<ReporteVisita> ReportesVisita => Set<ReporteVisita>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Nombre = "Admin", Correo = "admin@demo.local", Rol = "Administrador", Contrasena = "admin123" },
                new Usuario { Id = 2, Nombre = "Super", Correo = "sup@demo.local", Rol = "Supervisor", Contrasena = "sup123" },
                new Usuario { Id = 3, Nombre = "Tec", Correo = "tec@demo.local", Rol = "Tecnico", Contrasena = "tec123" }
            );

            mb.Entity<ConfiguracionSistema>().HasKey(c => c.Clave);
        }
    }

    public static class Semilla
    {
        public static void DatosIniciales(AppDbContext db)
        {
            if (!db.Configuraciones.Any())
            {
                db.Configuraciones.AddRange(
                    new ConfiguracionSistema { Clave = "Reportes.ActivarEnvioAlCerrar", Valor = "true", Tipo = "bool", ActualizadoPor = "sistema" },
                    new ConfiguracionSistema { Clave = "Reportes.Formato", Valor = "HTML", Tipo = "string", ActualizadoPor = "sistema" },
                    new ConfiguracionSistema { Clave = "Reportes.AsuntoEmail", Valor = "Reporte de visita #{{IdVisita}}", Tipo = "string", ActualizadoPor = "sistema" },
                    new ConfiguracionSistema { Clave = "Reportes.CuerpoEmail", Valor = "Adjuntamos el reporte de su visita.", Tipo = "text", ActualizadoPor = "sistema" },
                    new ConfiguracionSistema
                    {
                        Clave = "Reportes.PlantillaHtml",
                        Tipo = "text",
                        ActualizadoPor = "sistema",
                        Valor = "<html><body><h3>Reporte visita #{{IdVisita}}</h3><p>Cliente: {{Cliente.Nombre}}</p><p>Técnico: {{Tecnico.Nombre}}</p><p>Inicio: {{FechaInicio}}</p><p>Fin: {{FechaFin}}</p><p>Duración: {{Duracion}}</p><p>Notas: {{Notas}}</p><p>Mapa: {{UrlMapa}}</p></body></html>"
                    },
                    new ConfiguracionSistema { Clave = "Reportes.IncluirMapa", Valor = "true", Tipo = "bool", ActualizadoPor = "sistema" }
                );
                db.SaveChanges();
            }
        }
    }
}
