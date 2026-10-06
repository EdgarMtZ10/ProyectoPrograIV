using System;
using System.Windows.Forms;

namespace simulador
{
    [System.ComponentModel.DesignerCategory("Code")]
    public partial class FrmMenuPrincipal : Form
    {
        public FrmMenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            FrmProveedor pantalla = new FrmProveedor();
            pantalla.Show();
        }

        private void btnEntradaProductos_Click(object sender, EventArgs e)
        {
            FrmEntradaProducto pantalla = new FrmEntradaProducto();
            pantalla.Show();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            FrmCliente pantalla = new FrmCliente();
            pantalla.Show();
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            FrmCompraVerificador pantalla = new FrmCompraVerificador();
            pantalla.Show();
        }

        private void mantenimientoDeProvedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProveedor pantalla = new FrmProveedor();
            pantalla.Show();
        }

        private void entradaDeProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCompraAlmacen pantalla = new FrmCompraAlmacen();
            pantalla.Show();
        }

        private void mantenimientoDeClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCliente pantalla = new FrmCliente();
            pantalla.Show();
        }

        private void comprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCompraVerificador pantalla = new FrmCompraVerificador();
            pantalla.Show();
        }

        private void frmMenuPrincipalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEntradaProducto pantalla = new FrmEntradaProducto();
            pantalla.Show();
        }
    }
}
