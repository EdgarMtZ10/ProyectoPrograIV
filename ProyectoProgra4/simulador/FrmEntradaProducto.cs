using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace simulador
{
    [System.ComponentModel.DesignerCategory("Code")]

    public partial class FrmEntradaProducto : Form
    {

        private const string HOST_ALMACEN = "127.0.0.1";
        private const int PUERTO_ALMACEN = 5000;

        public FrmEntradaProducto()
        {
            InitializeComponent();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                string trama = ArmarTrama();
                string respuesta = EnviarTrama(trama);
                txtRespuesta.Text = respuesta;
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexion: " + ex.Message;
            }
        }

        /// <summary>
        /// Arma la trama de texto plano de 109 caracteres segun el
        /// formato de ALMACEN1: tipo(1) + noProducto(10) + nombre(90) + precio(8)
        /// </summary>
        private string ArmarTrama()
        {
            if (string.IsNullOrWhiteSpace(cmbTipoTransaccion.Text))
                throw new Exception("Seleccione el tipo de transaccion.");

            string tipoTransaccion = cmbTipoTransaccion.Text
                .StartsWith("Modif", StringComparison.OrdinalIgnoreCase) ? "2" : "1";

            // Rellena/corta el numero de producto a 10 caracteres
            string noProducto = txtNoProducto.Text.PadRight(10).Substring(0, 10);

            // Rellena/corta el nombre a 90 caracteres
            string nombre = txtNombre.Text.PadRight(90).Substring(0, 90);

            // El precio se manda como entero de 8 digitos, ultimos 2 son decimales
            // Ej: usuario escribe 17500.00 -> se manda como 01750000
            decimal precioDecimal;
            if (!decimal.TryParse(txtPrecio.Text, out precioDecimal))
            {
                throw new Exception("El precio no es un numero valido.");
            }
            long precioEntero = (long)(precioDecimal * 100);
            string precio = precioEntero.ToString().PadLeft(8, '0');

            return tipoTransaccion + noProducto + nombre + precio;
        }

        /// <summary>
        /// Envia la trama al socket del almacen y espera la respuesta.
        /// </summary>
        private string EnviarTrama(string trama)
        {
            using (TcpClient cliente = new TcpClient(HOST_ALMACEN, PUERTO_ALMACEN))
            using (NetworkStream flujo = cliente.GetStream())
            {
                // El almacen lee con readLine(), asi que se manda con salto de linea
                byte[] datosEnvio = Encoding.UTF8.GetBytes(trama + "\n");
                flujo.Write(datosEnvio, 0, datosEnvio.Length);

                byte[] buffer = new byte[4096];
                int bytesLeidos = flujo.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesLeidos).Trim();
            }
        }
    }
}
