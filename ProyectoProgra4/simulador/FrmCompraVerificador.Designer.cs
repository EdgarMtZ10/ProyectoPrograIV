using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador
{
    partial class FrmCompraVerificador
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Paleta de colores del formulario
        private static readonly Color ColorFondo = Color.FromArgb(243, 245, 249);
        private static readonly Color ColorPrimario = Color.FromArgb(37, 99, 235);
        private static readonly Color ColorPrimarioHover = Color.FromArgb(29, 78, 216);
        private static readonly Color ColorTexto = Color.FromArgb(31, 41, 55);
        private static readonly Color ColorSecundario = Color.FromArgb(107, 114, 128);

        // Crea una "tarjeta" blanca con un titulo.
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

        // Coloca una etiqueta con su caja de texto debajo, dentro de una tarjeta.
        private void ConfigurarCampo(Panel tarjeta, Label etiqueta, TextBox caja,
                                     string texto, int x, int tabIndex)
        {
            etiqueta.AutoSize = true;
            etiqueta.Text = texto;
            etiqueta.Font = new Font("Segoe UI", 9F);
            etiqueta.ForeColor = ColorSecundario;
            etiqueta.Location = new Point(x, 46);

            caja.Font = new Font("Segoe UI", 10.5F);
            caja.ForeColor = ColorTexto;
            caja.BorderStyle = BorderStyle.FixedSingle;
            caja.Location = new Point(x, 68);
            caja.Size = new Size(216, 27);
            caja.TabIndex = tabIndex;

            tarjeta.Controls.Add(etiqueta);
            tarjeta.Controls.Add(caja);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnVolver = new System.Windows.Forms.Button();
            this.txtNumeroCompra = new System.Windows.Forms.TextBox();
            this.txtIdCliente = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtNumeroTarjeta = new System.Windows.Forms.TextBox();
            this.txtFechaVencimiento = new System.Windows.Forms.TextBox();
            this.txtCodigoVerificacion = new System.Windows.Forms.TextBox();
            this.txtRespuesta = new System.Windows.Forms.TextBox();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.SuspendLayout();

            // ---------- Encabezado ----------
            Panel panelEncabezado = new Panel();
            panelEncabezado.BackColor = ColorPrimario;
            panelEncabezado.Dock = DockStyle.Top;
            panelEncabezado.Height = 80;

            Label lblTitulo = new Label();
            lblTitulo.AutoSize = true;
            lblTitulo.Text = "Registro de compras";
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(24, 12);

            Label lblSubtitulo = new Label();
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Text = "Simulador Nova Online - los datos de pago se envían cifrados (AES)";
            lblSubtitulo.Font = new Font("Segoe UI", 9.5F);
            lblSubtitulo.ForeColor = Color.FromArgb(219, 234, 254);
            lblSubtitulo.Location = new Point(27, 50);

            // ---------- Botón Volver al Menú ----------
            this.btnVolver.Text = "Volver al Menú";
            this.btnVolver.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.btnVolver.ForeColor = ColorPrimario;
            this.btnVolver.BackColor = Color.White;
            this.btnVolver.FlatStyle = FlatStyle.Flat;
            this.btnVolver.FlatAppearance.BorderSize = 0;
            this.btnVolver.Cursor = Cursors.Hand;
            this.btnVolver.Location = new Point(590, 20);
            this.btnVolver.Size = new Size(140, 40);
            this.btnVolver.TabIndex = 10;
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);

            panelEncabezado.Controls.Add(lblTitulo);
            panelEncabezado.Controls.Add(lblSubtitulo);
            panelEncabezado.Controls.Add(this.btnVolver);

            // ---------- Tarjeta 1: datos de la compra ----------
            Panel tarjetaCompra = CrearTarjeta("Datos de la compra", 100, 112);
            ConfigurarCampo(tarjetaCompra, this.label1, this.txtNumeroCompra, "Número de compra", 16, 0);
            ConfigurarCampo(tarjetaCompra, this.label2, this.txtIdCliente, "Identificador del cliente (cédula)", 248, 1);
            ConfigurarCampo(tarjetaCompra, this.label3, this.txtTotal, "Total de compra (ej. 15000.00)", 480, 2);

            // ---------- Tarjeta 2: datos de pago ----------
            Panel tarjetaPago = CrearTarjeta("Datos de pago", 224, 112);
            ConfigurarCampo(tarjetaPago, this.label4, this.txtNumeroTarjeta, "Número de tarjeta (se envía cifrado)", 16, 3);
            ConfigurarCampo(tarjetaPago, this.label5, this.txtFechaVencimiento, "Fecha de vencimiento (MM/AA)", 248, 4);
            ConfigurarCampo(tarjetaPago, this.label6, this.txtCodigoVerificacion, "CVV (3 dígitos, se envía cifrado)", 480, 5);

            // ---------- Tarjeta 3: productos (ahora una grilla, varios productos) ----------
            Panel tarjetaProductos = CrearTarjeta("Productos", 348, 196);

            Label lblPista = new Label();
            lblPista.AutoSize = true;
            lblPista.Text = "Escriba el número de producto (ej. 3444550000) y la cantidad. Use la última fila vacía para agregar más productos.";
            lblPista.Font = new Font("Segoe UI", 9F);
            lblPista.ForeColor = ColorSecundario;
            lblPista.Location = new Point(16, 36);
            tarjetaProductos.Controls.Add(lblPista);

            this.dgvProductos.Location = new Point(16, 60);
            this.dgvProductos.Size = new Size(680, 124);
            this.dgvProductos.AllowUserToAddRows = true;
            this.dgvProductos.AllowUserToResizeRows = false;
            this.dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.BackgroundColor = Color.White;
            this.dgvProductos.BorderStyle = BorderStyle.FixedSingle;
            this.dgvProductos.RowHeadersVisible = false;
            this.dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvProductos.GridColor = Color.FromArgb(229, 231, 235);
            this.dgvProductos.EnableHeadersVisualStyles = false;
            this.dgvProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvProductos.ColumnHeadersHeight = 32;
            this.dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = ColorPrimario;
            this.dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.dgvProductos.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColorPrimario;
            this.dgvProductos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            this.dgvProductos.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            this.dgvProductos.DefaultCellStyle.ForeColor = ColorTexto;
            this.dgvProductos.DefaultCellStyle.BackColor = Color.White;
            this.dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            this.dgvProductos.DefaultCellStyle.SelectionForeColor = ColorTexto;
            this.dgvProductos.RowTemplate.Height = 28;
            this.dgvProductos.TabIndex = 6;
            this.dgvProductos.Columns.Add("colProducto", "No. Producto");
            this.dgvProductos.Columns.Add("colCantidad", "Cantidad");
            tarjetaProductos.Controls.Add(this.dgvProductos);

            // ---------- Respuesta ----------
            this.label9.AutoSize = true;
            this.label9.Text = "Respuesta del verificador";
            this.label9.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.label9.ForeColor = ColorTexto;
            this.label9.Location = new Point(24, 560);

            this.txtRespuesta.Multiline = true;
            this.txtRespuesta.ReadOnly = true;
            this.txtRespuesta.ScrollBars = ScrollBars.Vertical;
            this.txtRespuesta.BackColor = Color.White;
            this.txtRespuesta.ForeColor = ColorTexto;
            this.txtRespuesta.Font = new Font("Segoe UI", 10F);
            this.txtRespuesta.BorderStyle = BorderStyle.FixedSingle;
            this.txtRespuesta.Location = new Point(24, 582);
            this.txtRespuesta.Size = new Size(560, 72);
            this.txtRespuesta.TabStop = false;

            // ---------- Boton Enviar ----------
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnEnviar.ForeColor = Color.White;
            this.btnEnviar.BackColor = ColorPrimario;
            this.btnEnviar.FlatStyle = FlatStyle.Flat;
            this.btnEnviar.FlatAppearance.BorderSize = 0;
            this.btnEnviar.FlatAppearance.MouseOverBackColor = ColorPrimarioHover;
            this.btnEnviar.Cursor = Cursors.Hand;
            this.btnEnviar.Location = new Point(600, 582);
            this.btnEnviar.Size = new Size(136, 72);
            this.btnEnviar.TabIndex = 7;
            this.btnEnviar.UseVisualStyleBackColor = false;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

            // ---------- Formulario ----------
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = ColorFondo;
            this.ClientSize = new Size(760, 670);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmCompraVerificador";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Registro de Compras";

            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtRespuesta);
            this.Controls.Add(this.label9);
            this.Controls.Add(tarjetaProductos);
            this.Controls.Add(tarjetaPago);
            this.Controls.Add(tarjetaCompra);
            this.Controls.Add(panelEncabezado);

            this.Load += new System.EventHandler(this.FrmCompraVerificador_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.TextBox txtNumeroCompra;
        private System.Windows.Forms.TextBox txtIdCliente;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtNumeroTarjeta;
        private System.Windows.Forms.TextBox txtFechaVencimiento;
        private System.Windows.Forms.TextBox txtCodigoVerificacion;
        private System.Windows.Forms.TextBox txtRespuesta;
        private System.Windows.Forms.DataGridView dgvProductos;
    }
}