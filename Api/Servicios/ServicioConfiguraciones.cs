using Api.Datos;
using Api.Modelos;

namespace Api.Servicios
{
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ServicioConfiguraciones
    {
        private readonly AppDbContext _db;
        private readonly Dictionary<string, (string v, string t, DateTime exp)> _cache = new();
        private readonly TimeSpan _ttl = TimeSpan.FromSeconds(60);

        public ServicioConfiguraciones(AppDbContext db) { _db = db; }

        public (string valor, string tipo)? Obtener(string clave)
        {
            if (_cache.TryGetValue(clave, out var c) && c.exp > DateTime.UtcNow) return (c.v, c.t);

            ConfiguracionSistema? conf = _db.Configuraciones.Find(clave);
            if (conf == null) return null;

            _cache[clave] = (conf.Valor, conf.Tipo, DateTime.UtcNow.Add(_ttl));
            return (conf.Valor, conf.Tipo);
        }

        public bool ObtenerBool(string clave, bool porDefecto = false)
        {
            var r = Obtener(clave);
            if (r == null) return porDefecto;
            bool b;
            if (bool.TryParse(r.Value.valor, out b)) return b;
            return porDefecto;
        }

        public string ObtenerStr(string clave, string porDefecto = "")
        {
            var r = Obtener(clave);
            return r?.valor ?? porDefecto;
        }

        public void Guardar(string clave, string valor, string tipo, string usuario)
        {
            ConfiguracionSistema? c = _db.Configuraciones.Find(clave);
            if (c == null)
            {
                c = new ConfiguracionSistema { Clave = clave, Valor = valor, Tipo = tipo, ActualizadoPor = usuario, ActualizadoEn = DateTime.UtcNow };
                _db.Configuraciones.Add(c);
            }
            else
            {
                c.Valor = valor; c.Tipo = tipo; c.ActualizadoPor = usuario; c.ActualizadoEn = DateTime.UtcNow;
                _db.Configuraciones.Update(c);
            }
            _db.SaveChanges();
            _cache.Remove(clave);
        }
    }
}
