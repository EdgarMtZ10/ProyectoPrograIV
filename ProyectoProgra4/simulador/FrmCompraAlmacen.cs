using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulador
{
    public class FrmCompraAlmacen: Form
    {
        private const string HOST_ALMACEN = "127.0.0.1";
        private const int PUERTO_ALMACEN = 5000;

        private TextBox txtNumeroIngreso;
        private TextBox txtCedula;
        private TextBox txtRespuesta;
        private DateTimePicker dtpFecha;
        private DataGridView dgvProductos;
        private Button btnEnviar;

        public FrmCompraAlmacen()
        {
            Text = "Entrada de productos (Compra)";
            ClientSize = new Size(480, 470);
            StartPosition = FormStartPosition.CenterParent;

            AgregarEtiqueta("Numero de ingreso:", 15);
            txtNumeroIngreso = new TextBox { Left = 170, Top = 12, Width = 280 };

            AgregarEtiqueta("Fecha de compra:", 50);
            dtpFecha = new DateTimePicker
            {
                Left = 170,
                Top = 47,
                Width = 280,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };

            AgregarEtiqueta("Cedula juridica:", 85);
            txtCedula = new TextBox { Left = 170, Top = 82, Width = 280 };

            AgregarEtiqueta("Productos:", 120);
            dgvProductos = new DataGridView
            {
                Left = 15,
                Top = 140,
                Width = 450,
                Height = 170,
                AllowUserToAddRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvProductos.Columns.Add("colProducto", "No. Producto");
            dgvProductos.Columns.Add("colCantidad", "Cantidad");

            btnEnviar = new Button { Text = "Enviar", Left = 15, Top = 320, Width = 450, Height = 30 };
            btnEnviar.Click += BtnEnviar_Click;

            AgregarEtiqueta("Respuesta:", 360);
            txtRespuesta = new TextBox
            {
                Left = 15,
                Top = 380,
                Width = 450,
                Height = 70,
                Multiline = true,
                ReadOnly = true
            };

            Controls.AddRange(new Control[]
            {
                txtNumeroIngreso, dtpFecha, txtCedula, dgvProductos, btnEnviar, txtRespuesta
            });
        }

        private void AgregarEtiqueta(string texto, int top)
        {
            Controls.Add(new Label { Text = texto, Left = 15, Top = top, AutoSize = true });
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {
            btnEnviar.Enabled = false;
            try
            {
                string trama = ArmarTrama();
                string respuesta = await Task.Run(() => EnviarTrama(trama));
                txtRespuesta.Text = Traducir(respuesta);
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error: " + ex.Message;
            }
            finally
            {
                btnEnviar.Enabled = true;
            }
        }

        // tipo(1) + numeroIngreso(10) + fecha(10) + cedula(10)
        // + por cada producto: noProducto(10) + cantidad(7)
        private string ArmarTrama()
        {
            var sb = new StringBuilder();
            sb.Append("5");
            sb.Append(txtNumeroIngreso.Text.Trim().PadLeft(10, '0'));
            sb.Append(dtpFecha.Value.ToString("yyyyMMdd").PadRight(10));
            sb.Append(txtCedula.Text.Trim().PadLeft(10, '0'));

            int lineas = 0;
            foreach (DataGridViewRow fila in dgvProductos.Rows)
            {
                if (fila.IsNewRow) continue;
                string producto = (fila.Cells[0].Value ?? "").ToString().Trim();
                string cantidad = (fila.Cells[1].Value ?? "").ToString().Trim();
                if (producto == "" && cantidad == "") continue;

                sb.Append(producto.PadLeft(10, '0'));
                sb.Append(cantidad.PadLeft(7, '0'));
                lineas++;
            }

            if (lineas == 0)
                throw new Exception("Agregue al menos un producto.");

            return sb.ToString();
        }

        private string EnviarTrama(string trama)
        {
            using (TcpClient cliente = new TcpClient(HOST_ALMACEN, PUERTO_ALMACEN))
            {
                cliente.ReceiveTimeout = 5000;
                using (NetworkStream flujo = cliente.GetStream())
                {
                    // El almacen lee con readLine(), por eso el salto de linea
                    byte[] datos = Encoding.UTF8.GetBytes(trama + "\n");
                    flujo.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[4096];
                    int leidos = flujo.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, leidos).Trim();
                }
            }
        }

        private string Traducir(string respuesta)
        {
            switch (respuesta.ToUpperInvariant())
            {
                case "EXITOSO":
                    return "La compra se registro correctamente y el inventario fue actualizado.";
                case "DATO INVALIDO":
                case "DATO INVÁLIDO":
                    return "Alguno de los datos no es valido (revise numero de ingreso, fecha, cedula, productos y cantidades).";
                case "PROVEEDOR INVALIDO":
                case "PROVEEDOR INVÁLIDO":
                    return "El proveedor no existe.";
                case "PRODUCTO INVALIDO":
                case "PRODUCTO INVÁLIDO":
                    return "Alguno de los productos no existe.";
                case "ERROR":
                    return "Ocurrio un error al procesar la compra.";
                default:
                    return respuesta;
            }
        }
    }
}
