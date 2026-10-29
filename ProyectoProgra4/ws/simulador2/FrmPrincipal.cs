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
            // 1. Configuración de la ventana maestra
            this.Text = "Simulador V2 - Módulos del Almacén";
            this.IsMdiContainer = true; // Convierte el formulario en un contenedor para otras ventanas
            this.WindowState = FormWindowState.Maximized; // Inicia maximizado

            // 2. Creación del menú superior
            MenuStrip menuSuperior = new MenuStrip();

            // 3. Opción principal del menú
            ToolStripMenuItem menuAlmacen = new ToolStripMenuItem("Mantenimiento Almacén");

            // 4. Submenús para cada pantalla
            ToolStripMenuItem itemProducto = new ToolStripMenuItem("1. Productos");
            itemProducto.Click += (sender, e) => AbrirFormulario<FrmProducto>();

            ToolStripMenuItem itemProveedor = new ToolStripMenuItem("2. Proveedores");
            itemProveedor.Click += (sender, e) => AbrirFormulario<FrmProveedor>();

            ToolStripMenuItem itemCompra = new ToolStripMenuItem("3. Registro de Compras");
            itemCompra.Click += (sender, e) => AbrirFormulario<FrmCompraAlmacen>();

            // 5. Ensamblar el menú
            menuAlmacen.DropDownItems.Add(itemProducto);
            menuAlmacen.DropDownItems.Add(itemProveedor);
            menuAlmacen.DropDownItems.Add(itemCompra);

            menuSuperior.Items.Add(menuAlmacen);

            // 6. Agregar el menú al formulario
            this.Controls.Add(menuSuperior);
            this.MainMenuStrip = menuSuperior;
        }

        // Método genérico para abrir formularios hijos.
        // Revisa si la ventana ya está abierta para traerla al frente en lugar de duplicarla.
        private void AbrirFormulario<T>() where T : Form, new()
        {
            foreach (Form formularioAbierto in this.MdiChildren)
            {
                if (formularioAbierto.GetType() == typeof(T))
                {
                    formularioAbierto.Activate();
                    return;
                }
            }

            T nuevoFormulario = new T();
            nuevoFormulario.MdiParent = this; // Le indica que FrmPrincipal es su contenedor
            nuevoFormulario.Show();
        }
    }
}