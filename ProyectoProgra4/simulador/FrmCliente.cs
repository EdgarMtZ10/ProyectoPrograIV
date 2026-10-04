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
    public partial class FrmCliente : Form
    {

        private const string HOST_VERIFICADOR = "127.0.0.1";
        private const int PUERTO_VERIFICADOR = 5001;

        public FrmCliente()
        {
            InitializeComponent();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                string json = ArmarJson();
                string respuesta = EnviarTrama(json);
                txtRespuesta.Text = respuesta;
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexion: " + ex.Message;
            }
        }

        private string ArmarJson()
        {
            string transaccion = cmbTransaccion.SelectedItem?.ToString() ?? "agregar";

            return "{"
                + "\"identificacion\": \"" + txtIdentificacion.Text + "\","
                + "\"pais\": \"" + txtPais.Text + "\","
                + "\"nombre\": \"" + txtNombre.Text + "\","
                + "\"primer_apellido\": \"" + txtPrimerApellido.Text + "\","
                + "\"segundo_apellido\": \"" + txtSegundoApellido.Text + "\","
                + "\"correo\": \"" + txtCorreo.Text + "\","
                + "\"telefono\": \"" + txtTelefono.Text + "\","
                + "\"direccion\": \"" + txtDireccion.Text + "\","
                + "\"transaccion\": \"" + transaccion + "\""
                + "}";
        }

        private string EnviarTrama(string trama)
        {
            using (TcpClient cliente = new TcpClient(HOST_VERIFICADOR, PUERTO_VERIFICADOR))
            using (NetworkStream flujo = cliente.GetStream())
            {
                // El verificador usa recv()/sendall() sin esperar salto de linea
                byte[] datosEnvio = Encoding.UTF8.GetBytes(trama);
                flujo.Write(datosEnvio, 0, datosEnvio.Length);

                byte[] buffer = new byte[4096];
                int bytesLeidos = flujo.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesLeidos).Trim();
            }
        }
    }
}
