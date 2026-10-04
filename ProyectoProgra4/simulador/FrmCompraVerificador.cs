using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulador
{
    public partial class FrmCompraVerificador : Form
    {

        private const string HOST_VERIFICADOR = "127.0.0.1";
        private const int PUERTO_VERIFICADOR = 5001;


        // Llave y IV fijos para pruebas - deben coincidir con lo que
        // use el verificador para descifrar (confirmar con Jean).
        private static readonly byte[] LLAVE_AES = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes
        private static readonly byte[] IV_AES = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes

        public FrmCompraVerificador()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmCompraVerificador_Load(object sender, EventArgs e)
        {

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
            string tarjetaCifrada = Cifrar(txtNumeroTarjeta.Text);
            string fechaCifrada = Cifrar(txtFechaVencimiento.Text);
            string codigoCifrado = Cifrar(txtCodigoVerificacion.Text);

            string fechaHoy = DateTime.Now.ToString("yyyy-MM-dd");

            return "{"
         + "\"transaccion\": \"compra\","
         + "\"numero_compra\": \"" + txtNumeroCompra.Text + "\","
         + "\"identificador_cliente\": \"" + txtIdCliente.Text + "\","
         + "\"fecha_compra\": \"" + fechaHoy + "\","
         + "\"total_compra\": \"" + txtTotal.Text + "\","
         + "\"numero_tarjeta\": \"" + tarjetaCifrada + "\","
         + "\"fecha_vencimiento\": \"" + fechaCifrada + "\","
         + "\"codigo_verificacion\": \"" + codigoCifrado + "\","
         + "\"productos\": [{"
             + "\"codigo_producto\": \"" + txtNoProducto.Text + "\","
             + "\"cantidad\": \"" + txtCantidad.Text + "\""
         + "}]"
         + "}";
        }

        /// <summary>
        /// Cifra un texto con AES y lo devuelve en Base64.
        /// </summary>
        private string Cifrar(string textoPlano)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = LLAVE_AES;
                aes.IV = IV_AES;

                using (ICryptoTransform encriptador = aes.CreateEncryptor())
                {
                    byte[] datos = Encoding.UTF8.GetBytes(textoPlano);
                    byte[] cifrado = encriptador.TransformFinalBlock(datos, 0, datos.Length);
                    return Convert.ToBase64String(cifrado);
                }
            }
        }

        private string EnviarTrama(string trama)
        {
            using (TcpClient cliente = new TcpClient(HOST_VERIFICADOR, PUERTO_VERIFICADOR))
            using (NetworkStream flujo = cliente.GetStream())
            {
                byte[] datosEnvio = Encoding.UTF8.GetBytes(trama);
                flujo.Write(datosEnvio, 0, datosEnvio.Length);

                byte[] buffer = new byte[4096];
                int bytesLeidos = flujo.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesLeidos).Trim();
            }

        }
    }
}
