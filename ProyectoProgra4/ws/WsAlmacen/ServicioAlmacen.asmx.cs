using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Services;

namespace WsAlmacen
{
    [WebService(Namespace = "http://novaonline.cr/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ServicioAlmacen : WebService
    {
        [WebMethod]
        public string Ping()
        {
            return "WsAlmacen activo";
        }

        // mantenimiento de productos con una trama de 109 caracteres
        [WebMethod]
        public Respuesta MantenimientoProducto(string tipoTransaccion, string numeroProducto,
                                               string nombreProducto, decimal precio)
        {
            return Ejecutar("Problemas con el Producto. ", () =>
            {
                // El precio viaja en centavos, 8 dígitos: 17500.00 y se envía como 01750000
                decimal centavos = Math.Round(precio * 100, 0, MidpointRounding.AwayFromZero);
                if (centavos < 0 || centavos > 99999999)
                    throw new FormatException("Dato fuera de formato (precio).");

                return Campo(tipoTransaccion, 1, false, "tipo de transacción") +
                       Campo(numeroProducto, 10, true, "número de producto") +
                       Campo(nombreProducto, 90, false, "nombre del producto") +
                       centavos.ToString("0", CultureInfo.InvariantCulture).PadLeft(8, '0');
            });
        }

        // mantenimiento de proveedores (trama de 270 caracteres)
        [WebMethod]
        public Respuesta MantenimientoProveedor(string tipoTransaccion, string cedulaJuridica,
                                                string nombreEmpresa, string nombreContacto,
                                                string telefono, string correo, string estado)
        {
            return Ejecutar("Problemas con el proveedor. ", () =>
                Campo(tipoTransaccion, 1, false, "tipo de transacción") +
                Campo(cedulaJuridica, 10, true, "cédula jurídica") +
                Campo(nombreEmpresa, 100, false, "nombre de la empresa") +
                Campo(nombreContacto, 75, false, "nombre del contacto") +
                Campo(telefono, 8, false, "teléfono") +
                Campo(correo, 75, false, "correo electrónico") +
                Campo(estado, 1, false, "estado"));
        }

        // compra de productos (cabecera de 31 caracteres + 17 por producto)
        [WebMethod]
        public Respuesta RegistrarCompra(string numeroIngreso, string fechaCompra,
                                         string cedulaJuridica, List<ProductoCompra> listaProductos)
        {
            return Ejecutar("Problemas con la compra. ", () =>
            {
                if (listaProductos == null || listaProductos.Count == 0)
                    throw new FormatException("Debe indicar al menos un producto.");

                StringBuilder trama = new StringBuilder();
                trama.Append("5");
                trama.Append(Campo(numeroIngreso, 10, true, "número de ingreso"));
                trama.Append(Campo(fechaCompra, 10, false, "fecha de compra"));
                trama.Append(Campo(cedulaJuridica, 10, true, "cédula jurídica"));

                foreach (ProductoCompra producto in listaProductos)
                {
                    if (producto == null)
                        throw new FormatException("La lista de productos tiene una línea vacía.");

                    trama.Append(Campo(producto.NoProducto, 10, true, "número de producto"));
                    trama.Append(Campo(producto.Cantidad, 7, true, "cantidad"));
                }
                return trama.ToString();
            });
        }

        // Arma la trama, la envía al Almacén y traduce la respuesta a Resultado y Mensaje.
        // Cualquier fallo vuelve como Resultado = false, por lo que el servicio nunca lanza una excepción SOAP.
        private Respuesta Ejecutar(string prefijoError, Func<string> armarTrama)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                string respuestaAlmacen = ClienteSocket.EnviarTrama(armarTrama());

                if (respuestaAlmacen == "EXITOSO")
                {
                    respuesta.Resultado = true;
                    respuesta.Mensaje = "Exitoso";
                }
                else
                {
                    respuesta.Resultado = false;
                    respuesta.Mensaje = prefijoError + respuestaAlmacen;
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = false;
                respuesta.Mensaje = prefijoError + ex.Message;
            }
            return respuesta;
        }

        // Ajusta un dato al ancho de su campo. Los números se rellenan con ceros a la izquierda
        // y el texto con espacios a la derecha. Si el dato no cabe, falla
        private static string Campo(string valor, int ancho, bool numerico, string nombre)
        {
            valor = (valor ?? "").Trim();
            if (valor.Length > ancho)
                throw new FormatException("Dato fuera de formato (" + nombre + ").");

            return numerico ? valor.PadLeft(ancho, '0') : valor.PadRight(ancho);
        }
    }
}