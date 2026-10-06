using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simulador
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class FrmCompraAlmacen : Form
    {
        private const string HOST_ALMACEN = "127.0.0.1";
        private const int PUERTO_ALMACEN = 5000;

        private static readonly Color ColorFondo = Color.FromArgb(243, 245, 249);
        private static readonly Color ColorPrimario = Color.FromArgb(37, 99, 235);
        private static readonly Color ColorPrimarioHover = Color.FromArgb(29, 78, 216);
        private static readonly Color ColorTexto = Color.FromArgb(31, 41, 55);
        private static readonly Color ColorSecundario = Color.FromArgb(107, 114, 128);

        private TextBox txtNumeroIngreso;
        private TextBox txtCedula;
        private TextBox txtRespuesta;
        private DateTimePicker dtpFecha;
        private DataGridView dgvProductos;
        private Button btnEnviar;
        private Button btnVolver;

        public FrmCompraAlmacen()
        {
            SuspendLayout();

            // ---------- Encabezado ----------
            Panel panelEncabezado = CrearEncabezado("Entrada de productos",
                "Simulador Nova Online - registrar una compra de productos a un proveedor");

            // ---------- Botón Volver al Menú ----------
            btnVolver = new Button();
            btnVolver.Text = "Volver al Menú";
            btnVolver.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnVolver.ForeColor = ColorPrimario;
            btnVolver.BackColor = Color.White;
            btnVolver.FlatStyle = FlatStyle.Flat;
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Cursor = Cursors.Hand;
            btnVolver.Location = new Point(590, 20);
            btnVolver.Size = new Size(140, 40);
            btnVolver.TabIndex = 10;
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += BtnVolver_Click;

            panelEncabezado.Controls.Add(btnVolver);

            // ---------- Tarjeta 1: datos de la compra ----------
            Panel tarjetaCompra = CrearTarjeta("Datos de la compra", 100, 112);

            txtNumeroIngreso = new TextBox();
            AgregarCampo(tarjetaCompra, "Número de ingreso", txtNumeroIngreso, 16, 216, 0);

            dtpFecha = new DateTimePicker();
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.MaxDate = DateTime.Today;
            AgregarCampo(tarjetaCompra, "Fecha de compra", dtpFecha, 248, 216, 1);

            txtCedula = new TextBox();
            AgregarCampo(tarjetaCompra, "Cédula jurídica del proveedor", txtCedula, 480, 216, 2);

            // ---------- Tarjeta 2: productos ----------
            Panel tarjetaProductos = CrearTarjeta("Productos", 224, 212);

            Label lblPista = new Label();
            lblPista.AutoSize = true;
            lblPista.Text = "Escriba el número de producto y la cantidad. Use la última fila vacía para agregar más.";
            lblPista.Font = new Font("Segoe UI", 9F);
            lblPista.ForeColor = ColorSecundario;
            lblPista.Location = new Point(16, 36);
            tarjetaProductos.Controls.Add(lblPista);

            dgvProductos = new DataGridView();
            dgvProductos.Location = new Point(16, 60);
            dgvProductos.Size = new Size(680, 140);
            dgvProductos.AllowUserToAddRows = true;
            dgvProductos.AllowUserToResizeRows = false;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.FixedSingle;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProductos.GridColor = Color.FromArgb(229, 231, 235);
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProductos.ColumnHeadersHeight = 32;
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = ColorPrimario;
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvProductos.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColorPrimario;
            dgvProductos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvProductos.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvProductos.DefaultCellStyle.ForeColor = ColorTexto;
            dgvProductos.DefaultCellStyle.BackColor = Color.White;
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvProductos.DefaultCellStyle.SelectionForeColor = ColorTexto;
            dgvProductos.RowTemplate.Height = 28;
            dgvProductos.TabIndex = 3;
            dgvProductos.Columns.Add("colProducto", "No. Producto");
            dgvProductos.Columns.Add("colCantidad", "Cantidad");
            tarjetaProductos.Controls.Add(dgvProductos);

            // ---------- Respuesta ----------
            Label lblRespuesta = new Label();
            lblRespuesta.AutoSize = true;
            lblRespuesta.Text = "Respuesta del almacén";
            lblRespuesta.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblRespuesta.ForeColor = ColorTexto;
            lblRespuesta.Location = new Point(24, 476);

            txtRespuesta = new TextBox();
            txtRespuesta.Multiline = true;
            txtRespuesta.ReadOnly = true;
            txtRespuesta.ScrollBars = ScrollBars.Vertical;
            txtRespuesta.BackColor = Color.White;
            txtRespuesta.ForeColor = ColorTexto;
            txtRespuesta.Font = new Font("Segoe UI", 10F);
            txtRespuesta.BorderStyle = BorderStyle.FixedSingle;
            txtRespuesta.Location = new Point(24, 498);
            txtRespuesta.Size = new Size(560, 72);
            txtRespuesta.TabStop = false;

            // ---------- Boton Enviar ----------
            btnEnviar = new Button();
            btnEnviar.Text = "Enviar";
            btnEnviar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnEnviar.ForeColor = Color.White;
            btnEnviar.BackColor = ColorPrimario;
            btnEnviar.FlatStyle = FlatStyle.Flat;
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.FlatAppearance.MouseOverBackColor = ColorPrimarioHover;
            btnEnviar.Cursor = Cursors.Hand;
            btnEnviar.Location = new Point(600, 498);
            btnEnviar.Size = new Size(136, 72);
            btnEnviar.TabIndex = 4;
            btnEnviar.UseVisualStyleBackColor = false;
            btnEnviar.Click += BtnEnviar_Click;

            // ---------- Formulario ----------
            Text = "Entrada de productos (Compra)";
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = ColorFondo;
            ClientSize = new Size(760, 590);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            Controls.Add(btnEnviar);
            Controls.Add(txtRespuesta);
            Controls.Add(lblRespuesta);
            Controls.Add(tarjetaProductos);
            Controls.Add(tarjetaCompra);
            Controls.Add(panelEncabezado);

            ResumeLayout(false);
            PerformLayout();
        }

        // ================= AYUDAS DE DISEÑO =================

        private Panel CrearEncabezado(string titulo, string subtitulo)
        {
            Panel panel = new Panel();
            panel.BackColor = ColorPrimario;
            panel.Dock = DockStyle.Top;
            panel.Height = 80;

            Label lblTitulo = new Label();
            lblTitulo.AutoSize = true;
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(24, 12);

            Label lblSubtitulo = new Label();
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Text = subtitulo;
            lblSubtitulo.Font = new Font("Segoe UI", 9.5F);
            lblSubtitulo.ForeColor = Color.FromArgb(219, 234, 254);
            lblSubtitulo.Location = new Point(27, 50);

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblSubtitulo);
            return panel;
        }

        private Panel CrearTarjeta(string titulo, int y, int alto)
        {
            Panel tarjeta = new Panel();
            tarjeta.BackColor = Color.White;
            tarjeta.Location = new Point(24, y);
            tarjeta.Size = new Size(712, alto);

            Label lblTitulo = new Label();
            lblTitulo.AutoSize = true;
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitulo.ForeColor = ColorPrimario;
            lblTitulo.Location = new Point(16, 10);
            tarjeta.Controls.Add(lblTitulo);
            return tarjeta;
        }

        private void AgregarCampo(Panel tarjeta, string texto, Control campo,
                                int x, int ancho, int tabIndex)
        {
            Label etiqueta = new Label();
            etiqueta.AutoSize = true;
            etiqueta.Text = texto;
            etiqueta.Font = new Font("Segoe UI", 9F);
            etiqueta.ForeColor = ColorSecundario;
            etiqueta.Location = new Point(x, 46);

            campo.Font = new Font("Segoe UI", 10.5F);
            campo.ForeColor = ColorTexto;
            campo.Location = new Point(x, 68);
            campo.Width = ancho;
            campo.TabIndex = tabIndex;

            TextBox caja = campo as TextBox;
            if (caja != null)
            {
                caja.BorderStyle = BorderStyle.FixedSingle;
            }

            tarjeta.Controls.Add(etiqueta);
            tarjeta.Controls.Add(campo);
        }

        // ================= LOGICA =================

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            // Asume que tu menú principal se llama FrmMenuPrincipal
            FrmMenuPrincipal menu = new FrmMenuPrincipal();
            menu.Show();
            this.Close();
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