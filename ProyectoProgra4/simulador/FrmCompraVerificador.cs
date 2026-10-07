using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulador
{
    [System.ComponentModel.DesignerCategory("Code")]
    public partial class FrmCompraVerificador : Form
    {
        private const string HOST_VERIFICADOR = "127.0.0.1";
        private const int PUERTO_VERIFICADOR = 5001;

        // Llave y IV fijos para pruebas. El Verificador nunca descifra la tarjeta,
        // solo valida que parezca cifrada (Base64, multiplo de 16 bytes).
        private static readonly byte[] LLAVE_AES = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes
        private static readonly byte[] IV_AES = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes

        public FrmCompraVerificador()
        {
            InitializeComponent();
        }

        private void FrmCompraVerificador_Load(object sender, EventArgs e)
        {
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
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
            catch (InvalidOperationException ex)
            {
                // Error de uso (por ejemplo, no agregaron productos): no es un problema de conexion
                txtRespuesta.Text = ex.Message;
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

        private static string Esc(string s)
        {
            return (s ?? "").Trim().Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private string ArmarJson()
        {
            string tarjetaCifrada = Cifrar(txtNumeroTarjeta.Text);
            string fechaCifrada = Cifrar(txtFechaVencimiento.Text);
            string codigoCifrado = Cifrar(txtCodigoVerificacion.Text);

            string fechaHoy = DateTime.Now.ToString("yyyyMMdd");

            return "{"
                + "\"transaccion\": \"compra\","
                + "\"numero_compra\": \"" + Esc(txtNumeroCompra.Text) + "\","
                + "\"id_cliente\": \"" + Esc(txtIdCliente.Text) + "\","
                + "\"fecha_compra\": \"" + fechaHoy + "\","
                + "\"total_compra\": \"" + Esc(txtTotal.Text).Replace(',', '.') + "\","
                + "\"num_tarjeta\": \"" + tarjetaCifrada + "\","
                + "\"fecha_vencimiento\": \"" + fechaCifrada + "\","
                + "\"codigo_verificacion\": \"" + codigoCifrado + "\","
                + "\"productos\": " + ArmarProductosJson()
                + "}";
        }

        /// <summary>
        /// Lee todas las filas de la grilla y arma la lista de productos en JSON:
        /// [{"codigo_producto": "...", "cantidad": "..."}, {...}]
        /// Ignora filas vacias. Si no hay ningun producto, avisa al usuario.
        /// </summary>
        private string ArmarProductosJson()
        {
            // Confirma la celda que el usuario todavia estaba escribiendo
            dgvProductos.EndEdit();

            StringBuilder sb = new StringBuilder("[");
            int lineas = 0;

            foreach (DataGridViewRow fila in dgvProductos.Rows)
            {
                if (fila.IsNewRow) continue;

                string codigo = (fila.Cells[0].Value ?? "").ToString().Trim();
                string cantidad = (fila.Cells[1].Value ?? "").ToString().Trim();

                if (codigo == "" && cantidad == "") continue;

                if (lineas > 0) sb.Append(",");
                sb.Append("{\"codigo_producto\": \"" + Esc(codigo) + "\", \"cantidad\": \"" + Esc(cantidad) + "\"}");
                lineas++;
            }

            if (lineas == 0)
                throw new InvalidOperationException("Agregue al menos un producto a la compra.");

            sb.Append("]");
            return sb.ToString();
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

        /// <summary>
        /// Traduce el JSON crudo del Verificador ({"status": "OK"|"1"|"2"})
        /// a un mensaje entendible para quien prueba el simulador.
        /// </summary>
        private string Traducir(string respuestaJson)
        {
            Match coincidencia = Regex.Match(respuestaJson, "\"status\"\\s*:\\s*\"([^\"]+)\"");

            if (!coincidencia.Success)
            {
                return "Respuesta inesperada del verificador: " + respuestaJson;
            }

            string status = coincidencia.Groups[1].Value;

            switch (status)
            {
                case "OK":
                    return "La compra se registro correctamente.";
                case "1":
                    return "Los datos de la compra no son validos (revise numero de compra, cliente, tarjeta, productos y cantidades).";
                case "2":
                    return "El almacen rechazo la compra (verifique que los productos existan y tengan suficiente inventario).";
                default:
                    return "Respuesta desconocida del verificador: " + respuestaJson;
            }
        }
    }
}