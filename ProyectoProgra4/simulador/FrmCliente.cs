using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulador
{
    [System.ComponentModel.DesignerCategory("Code")]

    public partial class FrmCliente : Form
    {

        private const string HOST_VERIFICADOR = "127.0.0.1";
        private const int PUERTO_VERIFICADOR = 5001;

        public FrmCliente()
        {
            InitializeComponent();
        }

        private async void btnEnviar_Click(object sender, EventArgs e)
        {
            btnEnviar.Enabled = false;
            try
            {
                string json = ArmarJson();
                string respuesta = await Task.Run(() => EnviarTrama(json));
                txtRespuesta.Text = Traducir(respuesta);
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexion: " + ex.Message;
            }
            finally
            {
                btnEnviar.Enabled = true;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal menu = new FrmMenuPrincipal();
            menu.Show();
            this.Close(); // o this.Hide() dependiendo de tu lógica de ventanas
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

        private void FrmCliente_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Traduce el JSON crudo de Verificador1.py a un mensaje
        /// entendible. A diferencia de la compra (status siempre
        /// numerico), aqui el borrado con historial de compras devuelve
        /// un STRING completo en vez de un numero
        /// ("Error: cliente tiene historial de compras"), por eso la
        /// comparacion no puede ser solo switch de numeros -- primero
        /// se revisa ese caso especial de texto, y solo si no aplica se
        /// interpreta como codigo numerico.
        /// </summary>
        private string Traducir(string respuestaJson)
        {
            Match coincidencia = Regex.Match(respuestaJson, "\"status\"\\s*:\\s*\"([^\"]+)\"");

            if (!coincidencia.Success)
            {
                return "Respuesta inesperada del verificador: " + respuestaJson;
            }

            string status = coincidencia.Groups[1].Value;

            // Caso especial: el borrado de un cliente con compras asociadas
            // no devuelve un numero, devuelve este texto completo.
            if (status.StartsWith("Error: cliente tiene historial", StringComparison.OrdinalIgnoreCase))
            {
                return "No se puede borrar: el cliente tiene compras registradas.";
            }

            switch (status)
            {
                case "OK":
                    return "La operacion sobre el cliente se realizo correctamente.";
                case "1":
                    return "Los datos del cliente no son validos (revise identificacion, pais, nombre, correo, telefono o direccion).";
                case "2":
                    return "Ya existe un cliente con esa identificacion.";
                case "3":
                    return "No existe ningun cliente con esa identificacion.";
                case "4":
                    return "Ocurrio un error no controlado al procesar la solicitud.";
                default:
                    return "Respuesta desconocida del verificador: " + respuestaJson;
            }
        }
    }
}