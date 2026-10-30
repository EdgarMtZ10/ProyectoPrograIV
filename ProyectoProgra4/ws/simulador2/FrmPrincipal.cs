using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador2
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = "Nova Online - Sistema de Ventas y Distribución";
            this.IsMdiContainer = true;

            // Ventana en tamaño normal (no maximizada) centrada en la pantalla
            this.WindowState = FormWindowState.Normal;
            this.Size = new Size(950, 850);
            this.StartPosition = FormStartPosition.CenterScreen;

            foreach (Control control in this.Controls)
            {
                if (control is MdiClient)
                {
                    control.BackColor = SystemColors.ControlLight;
                    break;
                }
            }

            Panel panelMenu = new Panel();
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Width = 180;
            panelMenu.BackColor = Color.FromArgb(35, 45, 60);

            Label lblTitulo = new Label();
            lblTitulo.Text = "NOVA ONLINE";
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.AutoSize = false;
            lblTitulo.Width = 180;
            lblTitulo.Height = 70;
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            Button btnProducto = CrearBotonMenu("1. Productos", 70);
            btnProducto.Click += (sender, e) => AbrirFormulario<FrmProducto>();

            Button btnProveedor = CrearBotonMenu("2. Proveedores", 110);
            btnProveedor.Click += (sender, e) => AbrirFormulario<FrmProveedor>();

            Button btnCompra = CrearBotonMenu("3. Registro de Compras", 150);
            btnCompra.Click += (sender, e) => AbrirFormulario<FrmCompraAlmacen>();

            Button btnVerificador = CrearBotonMenu("4. Pago de Compra - Verificador", 190);
            btnVerificador.Click += (sender, e) => AbrirFormulario<FrmCompraVerificador>();

            panelMenu.Controls.Add(lblTitulo);
            panelMenu.Controls.Add(btnProducto);
            panelMenu.Controls.Add(btnProveedor);
            panelMenu.Controls.Add(btnCompra);
            panelMenu.Controls.Add(btnVerificador);

            this.Controls.Add(panelMenu);
        }

        private Button CrearBotonMenu(string texto, int posicionY)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.Location = new Point(0, posicionY);
            btn.Width = 180;
            btn.Height = 40;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 60, 75);
            btn.ForeColor = Color.White;
            btn.BackColor = Color.FromArgb(35, 45, 60);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(10, 0, 0, 0);
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            return btn;
        }

        private void AbrirFormulario<T>() where T : Form, new()
        {
            Form formularioEncontrado = null;

            foreach (Form formularioAbierto in this.MdiChildren)
            {
                if (formularioAbierto.GetType() == typeof(T))
                {
                    formularioEncontrado = formularioAbierto;
                }
                else
                {
                    formularioAbierto.Close();
                }
            }

            if (formularioEncontrado != null)
            {
                formularioEncontrado.Activate();
            }
            else
            {
                T nuevoFormulario = new T();
                nuevoFormulario.MdiParent = this;
                nuevoFormulario.StartPosition = FormStartPosition.CenterScreen;
                nuevoFormulario.Show();
            }
        }
    }
}