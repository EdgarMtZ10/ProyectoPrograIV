using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WsAlmacen
{
    public static class ClienteSocket
    {
        // Envía una trama al Almacén y devuelve su respuesta, o sea una línea de texto.
        // El "\n" final se agrega aquí porque el Almacén lee por líneas.
        public static string EnviarTrama(string trama)
        {
            string host = ConfigurationManager.AppSettings["HostAlmacen"];
            int puerto = int.Parse(ConfigurationManager.AppSettings["PuertoAlmacen"]);

            try
            {
                using (TcpClient cliente = new TcpClient(host, puerto))
                {
                    cliente.ReceiveTimeout = 10000;
                    cliente.SendTimeout = 10000;

                    using (NetworkStream flujo = cliente.GetStream())
                    {
                        byte[] datos = Encoding.UTF8.GetBytes(trama + "\n");
                        flujo.Write(datos, 0, datos.Length);

                        byte[] buffer = new byte[4096];
                        int leidos = flujo.Read(buffer, 0, buffer.Length);
                        if (leidos == 0)
                            throw new InvalidOperationException("El almacén cerró la conexión sin responder.");

                        return Encoding.UTF8.GetString(buffer, 0, leidos).Trim();
                    }
                }
            }
            catch (SocketException)
            {
                throw new InvalidOperationException("No se pudo conectar con el almacén. Verifique que esté encendido.");
            }
            catch (IOException)
            {
                throw new InvalidOperationException("El almacén no respondió a tiempo.");
            }
        }
    }
}