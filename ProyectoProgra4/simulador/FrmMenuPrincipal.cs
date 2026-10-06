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

        private void AbrirVentana(Form pantalla)
        {
            this.Hide(); // Oculta el menú principal en lugar de dejarlo acumulándose
            pantalla.ShowDialog(); // Muestra la subventana de forma modal
            this.Show(); // Al cerrar la subventana, el menú principal vuelve a aparecer limpio
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmProveedor());
        }

        private void btnEntradaProductos_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmEntradaProducto());
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmCliente());
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmCompraVerificador());
        }

        private void mantenimientoDeProvedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmProveedor());
        }

        private void entradaDeProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmCompraAlmacen());
        }

        private void mantenimientoDeClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmCliente());
        }

        private void comprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmCompraVerificador());
        }

        private void frmMenuPrincipalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirVentana(new FrmEntradaProducto());
        }
    }
}