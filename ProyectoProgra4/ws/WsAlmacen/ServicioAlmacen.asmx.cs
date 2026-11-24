using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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

        // Mantenimiento de productos con una trama de 109 caracteres
        [WebMethod]
        public Respuesta MantenimientoProducto(string tipoTransaccion, string numeroProducto,
                                               string nombreProducto, decimal precio)
        {
            return Ejecutar("Problemas con el Producto. ", () =>
            {
                decimal centavos = Math.Round(precio * 100m, 0, MidpointRounding.AwayFromZero);
                if (centavos < 0 || centavos > 99999999)
                    throw new FormatException("precio: debe estar entre 0.01 y 999999.99.");

                return Campo(tipoTransaccion, 1, false, "tipo de transaccion") +
                       CampoExactoNumerico(numeroProducto, 10, "numero de producto") +
                       Campo(nombreProducto, 90, false, "nombre del producto") +
                       centavos.ToString("0", CultureInfo.InvariantCulture).PadLeft(8, '0');
            });
        }

        // Mantenimiento de proveedores (trama de 270 caracteres)
        [WebMethod]
        public Respuesta MantenimientoProveedor(string tipoTransaccion, string cedulaJuridica,
                                                string nombreEmpresa, string nombreContacto,
                                                string telefono, string correo, string estado)
        {
            return Ejecutar("Problemas con el proveedor. ", () =>
                Campo(tipoTransaccion, 1, false, "tipo de transaccion") +
                CampoExactoNumerico(cedulaJuridica, 10, "cedula juridica") +
                Campo(nombreEmpresa, 100, false, "nombre de la empresa") +
                Campo(nombreContacto, 75, false, "nombre del contacto") +
                Campo(telefono, 8, false, "telefono") +
                Campo(correo, 75, false, "correo") +
                Campo(estado, 1, false, "estado"));
        }

        // Compra de productos (cabecera de 31 caracteres + 17 por producto)
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
                trama.Append(Campo(numeroIngreso, 10, true, "numero de ingreso"));
                trama.Append(Campo(fechaCompra, 10, false, "fecha de compra"));
                trama.Append(CampoExactoNumerico(cedulaJuridica, 10, "cedula juridica"));

                foreach (ProductoCompra producto in listaProductos)
                {
                    if (producto == null)
                        throw new FormatException("La lista de productos tiene una linea vacia.");

                    trama.Append(CampoExactoNumerico(producto.NoProducto, 10, "numero de producto"));
                    trama.Append(Campo(producto.Cantidad, 7, true, "cantidad"));
                }
                return trama.ToString();
            });
        }

        // Conserva aqui tu metodo RegistrarSalida si lo tienes en esta clase,
        // usando CampoExactoNumerico(producto.NoProducto, 10, "numero de producto")

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

        // Exige longitud exacta de digitos numericos sin rellenar en silencio
        private static string CampoExactoNumerico(string valor, int ancho, string nombre)
        {
            valor = (valor ?? "").Trim();
            if (!Regex.IsMatch(valor, @"^\d{" + ancho + "}$"))
                throw new FormatException(nombre + ": debe tener exactamente " + ancho + " digitos numericos.");

            return valor;
        }

        // Ajusta un dato al ancho de su campo
        private static string Campo(string valor, int ancho, bool numerico, string nombre)
        {
            valor = (valor ?? "").Trim();
            if (valor.Length > ancho)
                throw new FormatException(nombre + ": maximo " + ancho + " caracteres.");

            return numerico ? valor.PadLeft(ancho, '0') : valor.PadRight(ancho);
        }
    }
}