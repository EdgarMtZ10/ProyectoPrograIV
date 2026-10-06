using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador
{
    partial class FrmMenuPrincipal
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
        private static readonly Color ColorBotonFondo = Color.FromArgb(239, 246, 255);
        private static readonly Color ColorBotonHover = Color.FromArgb(219, 234, 254);

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

        private void CrearBoton(Panel tarjeta, string texto, int x, int tabIndex, EventHandler alHacerClic)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            boton.ForeColor = ColorPrimario;
            boton.BackColor = ColorBotonFondo;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.MouseOverBackColor = ColorBotonHover;
            boton.Cursor = Cursors.Hand;
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.Padding = new Padding(12, 0, 8, 0);
            boton.Location = new Point(x, 46);
            boton.Size = new Size(216, 80);
            boton.TabIndex = tabIndex;
            boton.UseVisualStyleBackColor = false;
            boton.Click += alHacerClic;
            tarjeta.Controls.Add(boton);
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mantenimientoDeProvedoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entradaDeProductosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mantenimientoDeClientesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comprasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frmMenuPrincipalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();

            // ---------- Barra de menu original (oculta, se conservan nombres y eventos) ----------
            this.mantenimientoDeProvedoresToolStripMenuItem.Name = "mantenimientoDeProvedoresToolStripMenuItem";
            this.mantenimientoDeProvedoresToolStripMenuItem.Text = "Mantenimiento de provedores";
            this.mantenimientoDeProvedoresToolStripMenuItem.Click += new System.EventHandler(this.mantenimientoDeProvedoresToolStripMenuItem_Click);

            this.entradaDeProductosToolStripMenuItem.Name = "entradaDeProductosToolStripMenuItem";
            this.entradaDeProductosToolStripMenuItem.Text = "Entrada de productos";
            this.entradaDeProductosToolStripMenuItem.Click += new System.EventHandler(this.entradaDeProductosToolStripMenuItem_Click);

            this.mantenimientoDeClientesToolStripMenuItem.Name = "mantenimientoDeClientesToolStripMenuItem";
            this.mantenimientoDeClientesToolStripMenuItem.Text = "Mantenimiento de clientes";
            this.mantenimientoDeClientesToolStripMenuItem.Click += new System.EventHandler(this.mantenimientoDeClientesToolStripMenuItem_Click);

            this.comprasToolStripMenuItem.Name = "comprasToolStripMenuItem";
            this.comprasToolStripMenuItem.Text = "Compras";
            this.comprasToolStripMenuItem.Click += new System.EventHandler(this.comprasToolStripMenuItem_Click);

            this.frmMenuPrincipalToolStripMenuItem.Name = "frmMenuPrincipalToolStripMenuItem";
            this.frmMenuPrincipalToolStripMenuItem.Text = "Mantenimiento de productos";
            this.frmMenuPrincipalToolStripMenuItem.Click += new System.EventHandler(this.frmMenuPrincipalToolStripMenuItem_Click);

            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mantenimientoDeProvedoresToolStripMenuItem,
                this.entradaDeProductosToolStripMenuItem,
                this.mantenimientoDeClientesToolStripMenuItem,
                this.comprasToolStripMenuItem,
                this.frmMenuPrincipalToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;

            // ---------- Encabezado ----------
            Panel panelEncabezado = CrearEncabezado("Menú principal",
                "Simulador Nova Online - elija la operación que desea probar");

            // ---------- Tarjeta Almacen ----------
            Panel tarjetaAlmacen = CrearTarjeta("Almacén", 100, 142);
            CrearBoton(tarjetaAlmacen, "Mantenimiento de proveedores", 16, 0,
                new System.EventHandler(this.mantenimientoDeProvedoresToolStripMenuItem_Click));
            CrearBoton(tarjetaAlmacen, "Mantenimiento de productos", 248, 1,
                new System.EventHandler(this.frmMenuPrincipalToolStripMenuItem_Click));
            CrearBoton(tarjetaAlmacen, "Entrada de productos (compra)", 480, 2,
                new System.EventHandler(this.entradaDeProductosToolStripMenuItem_Click));

            // ---------- Tarjeta Verificador ----------
            Panel tarjetaVerificador = CrearTarjeta("Verificador", 258, 142);
            CrearBoton(tarjetaVerificador, "Mantenimiento de clientes", 16, 3,
                new System.EventHandler(this.mantenimientoDeClientesToolStripMenuItem_Click));
            CrearBoton(tarjetaVerificador, "Compras", 248, 4,
                new System.EventHandler(this.comprasToolStripMenuItem_Click));

            // ---------- Formulario ----------
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = ColorFondo;
            this.ClientSize = new Size(760, 590);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMenuPrincipal";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Menu";

            this.Controls.Add(tarjetaVerificador);
            this.Controls.Add(tarjetaAlmacen);
            this.Controls.Add(panelEncabezado);
            this.Controls.Add(this.menuStrip1);

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mantenimientoDeProvedoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entradaDeProductosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mantenimientoDeClientesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comprasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frmMenuPrincipalToolStripMenuItem;
    }
}