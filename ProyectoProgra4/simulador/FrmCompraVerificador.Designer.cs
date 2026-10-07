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

        private static readonly Color ColorFondo = Color.FromArgb(243, 245, 249);
        private static readonly Color ColorPrimario = Color.FromArgb(37, 99, 235);
        private static readonly Color ColorPrimarioHover = Color.FromArgb(29, 78, 216);
        private static readonly Color ColorTexto = Color.FromArgb(31, 41, 55);
        private static readonly Color ColorSecundario = Color.FromArgb(107, 114, 128);

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

        private void ConfigurarCampo(Panel tarjeta, Label etiqueta, TextBox caja,
                                     string texto, int x, int fila, int ancho, int tabIndex)
        {
            etiqueta.AutoSize = true;
            etiqueta.Text = texto;
            etiqueta.Font = new Font("Segoe UI", 9F);
            etiqueta.ForeColor = ColorSecundario;
            etiqueta.Location = new Point(x, 46 + fila * 76);

            caja.Font = new Font("Segoe UI", 10.5F);
            caja.ForeColor = ColorTexto;
            caja.BorderStyle = BorderStyle.FixedSingle;
            caja.Location = new Point(x, 68 + fila * 76);
            caja.Size = new Size(ancho, 27);
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
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnVolver = new System.Windows.Forms.Button();
            this.txtNumeroCompra = new System.Windows.Forms.TextBox();
            this.txtIdCliente = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtNumeroTarjeta = new System.Windows.Forms.TextBox();
            this.txtFechaVencimiento = new System.Windows.Forms.TextBox();
            this.txtCodigoVerificacion = new System.Windows.Forms.TextBox();
            this.txtNoProducto = new System.Windows.Forms.TextBox();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.txtRespuesta = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

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

            Panel tarjetaCompra = CrearTarjeta("Datos de la compra", 100, 188);
            ConfigurarCampo(tarjetaCompra, this.label1, this.txtNumeroCompra, "Número de compra", 16, 0, 336, 0);
            ConfigurarCampo(tarjetaCompra, this.label2, this.txtIdCliente, "Identificación (cédula)", 360, 0, 336, 1);
            ConfigurarCampo(tarjetaCompra, this.label3, this.txtTotal, "Total (ej: 17500.00)", 16, 1, 336, 2);

            Panel tarjetaPago = CrearTarjeta("Datos de pago", 300, 188);
            ConfigurarCampo(tarjetaPago, this.label4, this.txtNumeroTarjeta, "Número de tarjeta (se envía cifrado)", 16, 0, 336, 3);
            ConfigurarCampo(tarjetaPago, this.label5, this.txtFechaVencimiento, "Fecha de vencimiento (MM/AA)", 360, 0, 336, 4);
            ConfigurarCampo(tarjetaPago, this.label6, this.txtCodigoVerificacion, "CVV (3 dígitos, se envía cifrado)", 16, 1, 336, 5);

            Panel tarjetaProducto = CrearTarjeta("Producto", 500, 112);
            ConfigurarCampo(tarjetaProducto, this.label7, this.txtNoProducto, "Número de producto (ej: 3444550000, 10 dígitos)", 16, 0, 336, 6);
            ConfigurarCampo(tarjetaProducto, this.label8, this.txtCantidad, "Cantidad", 360, 0, 336, 7);

            this.label9.AutoSize = true;
            this.label9.Text = "Respuesta del verificador";
            this.label9.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.label9.ForeColor = ColorTexto;
            this.label9.Location = new Point(24, 624);

            this.txtRespuesta.Multiline = true;
            this.txtRespuesta.ReadOnly = true;
            this.txtRespuesta.ScrollBars = ScrollBars.Vertical;
            this.txtRespuesta.BackColor = Color.White;
            this.txtRespuesta.ForeColor = ColorTexto;
            this.txtRespuesta.Font = new Font("Segoe UI", 10F);
            this.txtRespuesta.BorderStyle = BorderStyle.FixedSingle;
            this.txtRespuesta.Location = new Point(24, 646);
            this.txtRespuesta.Size = new Size(560, 72);
            this.txtRespuesta.TabStop = false;

            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnEnviar.ForeColor = Color.White;
            this.btnEnviar.BackColor = ColorPrimario;
            this.btnEnviar.FlatStyle = FlatStyle.Flat;
            this.btnEnviar.FlatAppearance.BorderSize = 0;
            this.btnEnviar.FlatAppearance.MouseOverBackColor = ColorPrimarioHover;
            this.btnEnviar.Cursor = Cursors.Hand;
            this.btnEnviar.Location = new Point(600, 646);
            this.btnEnviar.Size = new Size(136, 72);
            this.btnEnviar.TabIndex = 8;
            this.btnEnviar.UseVisualStyleBackColor = false;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = ColorFondo;
            this.ClientSize = new Size(760, 750);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmCompraVerificador";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Registro de Compras";

            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtRespuesta);
            this.Controls.Add(this.label9);
            this.Controls.Add(tarjetaProducto);
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
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.TextBox txtNumeroCompra;
        private System.Windows.Forms.TextBox txtIdCliente;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtNumeroTarjeta;
        private System.Windows.Forms.TextBox txtFechaVencimiento;
        private System.Windows.Forms.TextBox txtCodigoVerificacion;
        private System.Windows.Forms.TextBox txtNoProducto;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.TextBox txtRespuesta;
    }
}