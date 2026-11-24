using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WsVerificador
{
    public static class ClienteSocket
    {
        // Envía el JSON al Verificador y devuelve su respuesta.
        
        public static string EnviarJson(string json)
        {
            //se leen host y puerto del Web.config
            string host = ConfigurationManager.AppSettings["HostVerificador"];
            int puerto = int.Parse(ConfigurationManager.AppSettings["PuertoVerificador"]);

            try
            {
                using (TcpClient cliente = new TcpClient(host, puerto))
                {
                    cliente.ReceiveTimeout = 10000;
                    cliente.SendTimeout = 10000;

                    using (NetworkStream flujo = cliente.GetStream())
                    {
                        byte[] datos = Encoding.UTF8.GetBytes(json);
                        flujo.Write(datos, 0, datos.Length);

                        //el verificador responde y cierra la conexión
                        using (MemoryStream recibido = new MemoryStream())
                        {
                            byte[] buffer = new byte[4096];
                            int leidos;
                            while ((leidos = flujo.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                recibido.Write(buffer, 0, leidos);
                            }

                            if (recibido.Length == 0)
                                throw new InvalidOperationException("El verificador cerró la conexión sin responder.");

                            return Encoding.UTF8.GetString(recibido.ToArray()).Trim();
                        }
                    }
                }
            }
            catch (SocketException)
            {
                throw new InvalidOperationException("No se pudo conectar con el verificador. Verifique que esté encendido.");
            }
            catch (IOException)
            {
                throw new InvalidOperationException("El verificador no respondió a tiempo.");
            }
        }
    }
}