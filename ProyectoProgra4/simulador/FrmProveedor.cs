using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulador
{
    public partial class FrmProveedor : Form
    {
        private const string HOST_ALMACEN = "127.0.0.1";
        private const int PUERTO_ALMACEN = 5000;


        public FrmProveedor()
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
        /// Arma la trama de texto plano de 270 caracteres segun el
        /// formato de ALMACEN2: tipo(1) + cedula(10) + nombre(100) +
        /// contacto(75) + telefono(8) + correo(75) + estado(1)
        /// </summary>
        private string ArmarTrama()
        {
            string tipoTransaccion = cmbTipoTransaccion.Text
            .StartsWith("Modif", StringComparison.OrdinalIgnoreCase) ? "4" : "3";

            string cedulaJuridica = txtCedulaJuridica.Text.PadRight(10).Substring(0, 10);
            string nombre = txtNombre.Text.PadRight(100).Substring(0, 100);
            string nombreContacto = txtNombreContacto.Text.PadRight(75).Substring(0, 75);
            string telefono = txtTelefono.Text.PadRight(8).Substring(0, 8);
            string correo = txtCorreo.Text.PadRight(75).Substring(0, 75);

            // 1 = Activo, 2 = Inactivo
            string estado = cmbEstado.SelectedIndex == 0 ? "1" : "2";

            return tipoTransaccion + cedulaJuridica + nombre + nombreContacto
                 + telefono + correo + estado;
        }

        /// <summary>
        /// Envia la trama al socket del almacen y espera la respuesta.
        /// </summary>
        private string EnviarTrama(string trama)
        {
            using (TcpClient cliente = new TcpClient(HOST_ALMACEN, PUERTO_ALMACEN))
            using (NetworkStream flujo = cliente.GetStream())
            {
                byte[] datosEnvio = Encoding.UTF8.GetBytes(trama + "\n");
                flujo.Write(datosEnvio, 0, datosEnvio.Length);

                byte[] buffer = new byte[4096];
                int bytesLeidos = flujo.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesLeidos).Trim();
            }
        }

        private void FrmProveedor_Load(object sender, EventArgs e)
        {

        }
    }
}
