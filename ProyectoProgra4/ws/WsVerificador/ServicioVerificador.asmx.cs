using System;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Web.Services;

namespace WsVerificador
{
    [WebService(Namespace = "http://novaonline.cr/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class ServicioVerificador : WebService
    {
        private const string MsgExitoso = "Exitoso";
        private const string PrefijoCliente = "Problemas al incluir la información. ";
        private const string PrefijoCompra = "Problemas al registrar la compra. ";

        [WebMethod]
        public string Ping()
        {
            return "WsVerificador activo";
        }

        // WS_VERIFICADOR1: mantenimiento de clientes 
        [WebMethod]
        public Respuesta MantenimientoCliente(string identificacion, string siglasPais, string nombre,
                                              string primerApellido, string segundoApellido, string correo,
                                              string telefono, string direccion, string tipoTransaccion)
        {
            return Ejecutar(PrefijoCliente, () =>
            {
                //mismo JSON que arma FrmCliente del simulador
                string transaccion = (tipoTransaccion ?? "").Trim().ToLower();

                return "{"
                    + "\"identificacion\": \"" + Esc(identificacion) + "\","
                    + "\"pais\": \"" + Esc(siglasPais) + "\","
                    + "\"nombre\": \"" + Esc(nombre) + "\","
                    + "\"primer_apellido\": \"" + Esc(primerApellido) + "\","
                    + "\"segundo_apellido\": \"" + Esc(segundoApellido) + "\","
                    + "\"correo\": \"" + Esc(correo) + "\","
                    + "\"telefono\": \"" + Esc(telefono) + "\","
                    + "\"direccion\": \"" + Esc(direccion) + "\","
                    + "\"transaccion\": \"" + Esc(transaccion) + "\""
                    + "}";
            }, TraducirCliente);
        }

        // Arma el JSON, lo envía al Verificador y traduce la respuesta a Resultado y Mensaje
        
        private Respuesta Ejecutar(string prefijoError, Func<string> armarJson, Func<string, string> traducirError)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                string respuestaVerificador = ClienteSocket.EnviarJson(armarJson());
                string status = LeerStatus(respuestaVerificador);

                if (status == "OK")
                {
                    respuesta.Resultado = true;
                    respuesta.Mensaje = MsgExitoso;
                }
                else if (status == null)
                {
                    respuesta.Resultado = false;
                    respuesta.Mensaje = prefijoError + "Respuesta inesperada del verificador: " + respuestaVerificador;
                }
                else
                {
                    respuesta.Resultado = false;
                    respuesta.Mensaje = prefijoError + traducirError(status);
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = prefijoError + ex.Message;
            }
            return respuesta;
        }

        //se usa para no romper el json como se hacia en el simulador 
        private static string Esc(string s)
        {
            return (s ?? "").Trim().Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        //se saca el valor de "status" del JSON de respuesta
        private static string LeerStatus(string respuestaJson)
        {
            Match coincidencia = Regex.Match(respuestaJson ?? "", "\"status\"\\s*:\\s*\"([^\"]+)\"");
            return coincidencia.Success ? coincidencia.Groups[1].Value : null;
        }

        //textos de error de VERIFICADOR1 
        private static string TraducirCliente(string status)
        {
            if (status.StartsWith("Error: cliente", StringComparison.OrdinalIgnoreCase))
                return "No se puede borrar: el cliente tiene compras registradas.";

            switch (status)
            {
                case "1":
                    return "Los datos del cliente no son válidos (revise identificación, país, nombre, correo, teléfono o dirección).";
                case "2":
                    return "Ya existe un cliente con esa identificación.";
                case "3":
                    return "No existe ningún cliente con esa identificación.";
                case "4":
                    return "Ocurrió un error no controlado al procesar la solicitud.";
                default:
                    return "Respuesta desconocida del verificador (status " + status + ").";
            }
        }
    }
}