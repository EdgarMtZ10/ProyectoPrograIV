using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador
{
    partial class FrmEntradaProducto
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

        // Coloca una etiqueta con su caja de texto o combo debajo, dentro de una tarjeta.
        private void ConfigurarCampo(Panel tarjeta, Label etiqueta, Control caja,
                                     string texto, int x, int tabIndex)
        {
            etiqueta.AutoSize = true;
            etiqueta.Text = texto;
            etiqueta.Font = new Font("Segoe UI", 9F);
            etiqueta.ForeColor = ColorSecundario;
            etiqueta.Location = new Point(x, 46);

            caja.Font = new Font("Segoe UI", 10.5F);
            caja.ForeColor = ColorTexto;
            caja.Location = new Point(x, 68);
            caja.Size = new Size(216, 27);
            caja.TabIndex = tabIndex;

            if (caja is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }

            tarjeta.Controls.Add(etiqueta);
            tarjeta.Controls.Add(caja);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTipoTransaccion = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNoProducto = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnVolver = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRespuesta = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // ---------- Encabezado ----------
            Panel panelEncabezado = new Panel();
            panelEncabezado.BackColor = ColorPrimario;
            panelEncabezado.Dock = DockStyle.Top;
            panelEncabezado.Height = 80;

            Label lblTitulo = new Label();
            lblTitulo.AutoSize = true;
            lblTitulo.Text = "Mantenimiento de productos";
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(24, 12);

            Label lblSubtitulo = new Label();
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Text = "Simulador Nova Online - Gestión de inventario";
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

            // ---------- Tarjeta 1: Transacción ----------
            Panel tarjetaTransaccion = CrearTarjeta("Transacción", 100, 112);

            this.cmbTipoTransaccion.FormattingEnabled = true;
            this.cmbTipoTransaccion.Items.AddRange(new object[] {
            "Ingreso",
            "Modificación"});
            this.cmbTipoTransaccion.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigurarCampo(tarjetaTransaccion, this.label1, this.cmbTipoTransaccion, "Tipo de transacción", 16, 0);

            // ---------- Tarjeta 2: Datos del Producto ----------
            Panel tarjetaProducto = CrearTarjeta("Datos del Producto", 224, 112);
            ConfigurarCampo(tarjetaProducto, this.label2, this.txtNoProducto, "Número de producto", 16, 1);
            ConfigurarCampo(tarjetaProducto, this.label3, this.txtNombre, "Nombre del producto", 248, 2);
            ConfigurarCampo(tarjetaProducto, this.label4, this.txtPrecio, "Precio", 480, 3);

            // ---------- Respuesta ----------
            this.label5.AutoSize = true;
            this.label5.Text = "Respuesta del servidor";
            this.label5.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.label5.ForeColor = ColorTexto;
            this.label5.Location = new Point(24, 476);

            this.txtRespuesta.Multiline = true;
            this.txtRespuesta.ReadOnly = true;
            this.txtRespuesta.ScrollBars = ScrollBars.Vertical;
            this.txtRespuesta.BackColor = Color.White;
            this.txtRespuesta.ForeColor = ColorTexto;
            this.txtRespuesta.Font = new Font("Segoe UI", 10F);
            this.txtRespuesta.BorderStyle = BorderStyle.FixedSingle;
            this.txtRespuesta.Location = new Point(24, 498);
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
            this.btnEnviar.Location = new Point(600, 498);
            this.btnEnviar.Size = new Size(136, 72);
            this.btnEnviar.TabIndex = 4;
            this.btnEnviar.UseVisualStyleBackColor = false;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

            // ---------- Formulario ----------
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = ColorFondo;
            this.ClientSize = new System.Drawing.Size(760, 590);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmEntradaProducto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mantenimiento de Productos";

            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtRespuesta);
            this.Controls.Add(this.label5);
            this.Controls.Add(tarjetaProducto);
            this.Controls.Add(tarjetaTransaccion);
            this.Controls.Add(panelEncabezado);

            this.Load += new System.EventHandler(this.FrmEntradaProducto_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTipoTransaccion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNoProducto;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtRespuesta;
    }
}