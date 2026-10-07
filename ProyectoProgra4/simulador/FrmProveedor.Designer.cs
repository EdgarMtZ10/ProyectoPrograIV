using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador
{
    partial class FrmProveedor
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

        private void ConfigurarCampo(Panel tarjeta, Label etiqueta, Control campo,
                                     string texto, int x, int fila, int ancho, int tabIndex)
        {
            etiqueta.AutoSize = true;
            etiqueta.Text = texto;
            etiqueta.Font = new Font("Segoe UI", 9F);
            etiqueta.ForeColor = ColorSecundario;
            etiqueta.Location = new Point(x, 46 + fila * 76);

            campo.Font = new Font("Segoe UI", 10.5F);
            campo.ForeColor = ColorTexto;
            campo.Location = new Point(x, 68 + fila * 76);
            campo.Width = ancho;
            campo.TabIndex = tabIndex;

            TextBox caja = campo as TextBox;
            if (caja != null)
            {
                caja.BorderStyle = BorderStyle.FixedSingle;
            }

            ComboBox combo = campo as ComboBox;
            if (combo != null)
            {
                combo.DropDownStyle = ComboBoxStyle.DropDownList;
                combo.FlatStyle = FlatStyle.Flat;
                combo.BackColor = Color.White;
            }

            tarjeta.Controls.Add(etiqueta);
            tarjeta.Controls.Add(campo);
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
            this.cmbTipoTransaccion = new System.Windows.Forms.ComboBox();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.txtCedulaJuridica = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtNombreContacto = new System.Windows.Forms.TextBox();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.txtRespuesta = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.btnVolver = new System.Windows.Forms.Button();
            this.SuspendLayout();

            Panel panelEncabezado = CrearEncabezado("Mantenimiento de proveedores",
                "Simulador Nova Online - ingreso y modificación de proveedores en el almacén");

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

            panelEncabezado.Controls.Add(this.btnVolver);

            this.cmbTipoTransaccion.Items.AddRange(new object[] {
                "Ingreso",
                "Modificacion"});

            this.cmbEstado.Items.AddRange(new object[] {
                "Activo",
                "Inactivo"});

            Panel tarjetaOperacion = CrearTarjeta("Operación", 100, 188);
            ConfigurarCampo(tarjetaOperacion, this.label1, this.cmbTipoTransaccion, "Tipo de transacción", 16, 0, 336, 0);
            ConfigurarCampo(tarjetaOperacion, this.label2, this.txtCedulaJuridica, "Cédula jurídica (ej: 3100550000, 10 dígitos)", 360, 0, 336, 1);
            ConfigurarCampo(tarjetaOperacion, this.label7, this.cmbEstado, "Estado", 16, 1, 336, 2);

            Panel tarjetaDatos = CrearTarjeta("Datos del proveedor", 300, 188);
            ConfigurarCampo(tarjetaDatos, this.label3, this.txtNombre, "Nombre", 16, 0, 336, 3);
            ConfigurarCampo(tarjetaDatos, this.label4, this.txtNombreContacto, "Nombre del contacto (ej: Javier Fonseca Alvarado)", 360, 0, 336, 4);
            ConfigurarCampo(tarjetaDatos, this.label5, this.txtTelefono, "Teléfono (ej: 89403536, 8 dígitos)", 16, 1, 336, 5);
            ConfigurarCampo(tarjetaDatos, this.label6, this.txtCorreo, "Correo electrónico (ej: nombre@correo.com)", 360, 1, 336, 6);

            this.label8.AutoSize = true;
            this.label8.Text = "Respuesta del almacén";
            this.label8.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this.label8.ForeColor = ColorTexto;
            this.label8.Location = new Point(24, 500);

            this.txtRespuesta.Multiline = true;
            this.txtRespuesta.ReadOnly = true;
            this.txtRespuesta.ScrollBars = ScrollBars.Vertical;
            this.txtRespuesta.BackColor = Color.White;
            this.txtRespuesta.ForeColor = ColorTexto;
            this.txtRespuesta.Font = new Font("Segoe UI", 10F);
            this.txtRespuesta.BorderStyle = BorderStyle.FixedSingle;
            this.txtRespuesta.Location = new Point(24, 522);
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
            this.btnEnviar.Location = new Point(600, 522);
            this.btnEnviar.Size = new Size(136, 72);
            this.btnEnviar.TabIndex = 7;
            this.btnEnviar.UseVisualStyleBackColor = false;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = ColorFondo;
            this.ClientSize = new Size(760, 620);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmProveedor";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Mantenimiento de Proveedores";

            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtRespuesta);
            this.Controls.Add(this.label8);
            this.Controls.Add(tarjetaDatos);
            this.Controls.Add(tarjetaOperacion);
            this.Controls.Add(panelEncabezado);

            this.Load += new System.EventHandler(this.FrmProveedor_Load);
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
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.ComboBox cmbTipoTransaccion;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.TextBox txtCedulaJuridica;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtNombreContacto;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.TextBox txtRespuesta;
    }
}