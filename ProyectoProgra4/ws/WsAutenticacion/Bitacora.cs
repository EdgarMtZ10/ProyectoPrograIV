using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace WsAutenticacion
{
    public class Bitacora
    {
        private static readonly object Candado = new object();
        private static readonly JavaScriptSerializer Json = new JavaScriptSerializer();

        private static string Ruta()
        {
            string ruta = ConfigurationManager.AppSettings["RutaBitacora"];
            if (string.IsNullOrWhiteSpace(ruta)) ruta = "~/App_Data/bitacora_autenticacion.log";
            if (ruta.StartsWith("~")) ruta = HostingEnvironment.MapPath(ruta);
            return ruta;
        }

        public static void Registrar(string operacion, Dictionary<string, object> solicitud, Dictionary<string, object> respuesta)
        {
            try
            {
                string linea = "{\"fecha\":" + Json.Serialize(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                             + ",\"operacion\":" + Json.Serialize(operacion)
                             + ",\"solicitud\":" + Json.Serialize(solicitud)
                             + ",\"respuesta\":" + Json.Serialize(respuesta) + "}";

                string ruta = Ruta();
                string carpeta = Path.GetDirectoryName(ruta);
                if (!string.IsNullOrEmpty(carpeta)) Directory.CreateDirectory(carpeta);

                lock (Candado)
                {
                    File.AppendAllText(ruta, linea + Environment.NewLine, new UTF8Encoding(false));
                }
            }
            catch
            {
                // La bitácora nunca debe tumbar el servicio.
            }
        }
    }
}