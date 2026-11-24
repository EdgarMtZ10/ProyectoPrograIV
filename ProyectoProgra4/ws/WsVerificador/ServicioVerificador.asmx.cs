using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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

        // WS_VERIFICADOR2: registro de una compra y su pago.
        
        [WebMethod]
        public Respuesta RegistrarCompraCliente(string numeroCompra, string idCliente, string fechaCompra,
                                                string totalCompra, string numeroTarjeta, string fechaVencimiento,
                                                string codigoVerificacion, List<ProductoCompra> listaProductos)
        {
            return Ejecutar(PrefijoCompra, () =>
            {
                //se descifran los datos generales
                string compra = Descifrar(numeroCompra, "número de compra");
                string cliente = Descifrar(idCliente, "identificador del cliente");
                string fecha = Descifrar(fechaCompra, "fecha de compra");
                string total = Descifrar(totalCompra, "total de compra").Replace(',', '.');

                //los datos de la tarjeta solo se revisa el cifrado
                string tarjeta = ValidarCifrado(numeroTarjeta, "número de tarjeta");
                string vencimiento = ValidarCifrado(fechaVencimiento, "fecha de vencimiento");
                string codigo = ValidarCifrado(codigoVerificacion, "código de verificación");

                //mismo JSON que arma FrmCompraVerificador del simulador
                return "{"
                    + "\"transaccion\": \"compra\","
                    + "\"numero_compra\": \"" + Esc(compra) + "\","
                    + "\"id_cliente\": \"" + Esc(cliente) + "\","
                    + "\"fecha_compra\": \"" + Esc(fecha) + "\","
                    + "\"total_compra\": \"" + Esc(total) + "\","
                    + "\"num_tarjeta\": \"" + Esc(tarjeta) + "\","
                    + "\"fecha_vencimiento\": \"" + Esc(vencimiento) + "\","
                    + "\"codigo_verificacion\": \"" + Esc(codigo) + "\","
                    + "\"productos\": " + ArmarProductosJson(listaProductos)
                    + "}";
            }, TraducirCompra);
        }

        // Arma la lista de productos en JSON
        private static string ArmarProductosJson(List<ProductoCompra> listaProductos)
        {
            if (listaProductos == null || listaProductos.Count == 0)
                throw new FormatException("Debe indicar al menos un producto.");

            StringBuilder sb = new StringBuilder("[");
            int lineas = 0;

            foreach (ProductoCompra producto in listaProductos)
            {
                if (producto == null)
                    throw new FormatException("La lista de productos tiene una línea vacía.");

                string codigo = Descifrar(producto.CodigoProducto, "código de producto");
                string cantidad = Descifrar(producto.Cantidad, "cantidad");

                if (lineas > 0) sb.Append(",");
                sb.Append("{\"codigo_producto\": \"" + Esc(codigo) + "\", \"cantidad\": \"" + Esc(cantidad) + "\"}");
                lineas++;
            }

            sb.Append("]");
            return sb.ToString();
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

        
        private static string Esc(string s)
        {
            return (s ?? "").Trim().Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        //se saca el valor de "status" del JSON de respuesta, null si no viene
        private static string LeerStatus(string respuestaJson)
        {
            Match coincidencia = Regex.Match(respuestaJson ?? "", "\"status\"\\s*:\\s*\"([^\"]+)\"");
            return coincidencia.Success ? coincidencia.Groups[1].Value : null;
        }

        //descifra un dato recibido, si no viene cifrado con la llave del sistema falla con un mensaje 
        private static string Descifrar(string valorCifrado, string nombre)
        {
            if (string.IsNullOrWhiteSpace(valorCifrado))
                throw new FormatException("Falta el dato " + nombre + ".");
            try
            {
                return CifradoAes.Descifrar(valorCifrado.Trim()).Trim();
            }
            catch (Exception ex) when (ex is FormatException || ex is CryptographicException)
            {
                throw new FormatException("El dato " + nombre + " no viene cifrado correctamente.");
            }
        }

        //revisa que el dato se pueda descifrar, pero devuelve el valor cifrado original
        private static string ValidarCifrado(string valorCifrado, string nombre)
        {
            Descifrar(valorCifrado, nombre);
            return valorCifrado.Trim();
        }

        //textos de error de VERIFICADOR2 
        private static string TraducirCompra(string status)
        {
            switch (status)
            {
                case "1":
                    return "Los datos de la compra no son válidos (revise número de compra, cliente, fecha del día, total, tarjeta, productos y cantidades).";
                case "2":
                    return "El almacén rechazó la compra (verifique que los productos existan y tengan suficiente inventario).";
                default:
                    return "Respuesta desconocida del verificador (status " + status + ").";
            }
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