namespace simulador
{
    partial class FrmMenuPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
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
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.SlateGray;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mantenimientoDeProvedoresToolStripMenuItem,
            this.entradaDeProductosToolStripMenuItem,
            this.mantenimientoDeClientesToolStripMenuItem,
            this.comprasToolStripMenuItem,
            this.frmMenuPrincipalToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mantenimientoDeProvedoresToolStripMenuItem
            // 
            this.mantenimientoDeProvedoresToolStripMenuItem.Name = "mantenimientoDeProvedoresToolStripMenuItem";
            this.mantenimientoDeProvedoresToolStripMenuItem.Size = new System.Drawing.Size(179, 20);
            this.mantenimientoDeProvedoresToolStripMenuItem.Text = "Mantenimiento de provedores";
            this.mantenimientoDeProvedoresToolStripMenuItem.Click += new System.EventHandler(this.mantenimientoDeProvedoresToolStripMenuItem_Click);
            // 
            // entradaDeProductosToolStripMenuItem
            // 
            this.entradaDeProductosToolStripMenuItem.Name = "entradaDeProductosToolStripMenuItem";
            this.entradaDeProductosToolStripMenuItem.Size = new System.Drawing.Size(132, 20);
            this.entradaDeProductosToolStripMenuItem.Text = "Entrada de productos";
            this.entradaDeProductosToolStripMenuItem.Click += new System.EventHandler(this.entradaDeProductosToolStripMenuItem_Click);
            // 
            // mantenimientoDeClientesToolStripMenuItem
            // 
            this.mantenimientoDeClientesToolStripMenuItem.Name = "mantenimientoDeClientesToolStripMenuItem";
            this.mantenimientoDeClientesToolStripMenuItem.Size = new System.Drawing.Size(160, 20);
            this.mantenimientoDeClientesToolStripMenuItem.Text = "Mantenimiento de clientes";
            this.mantenimientoDeClientesToolStripMenuItem.Click += new System.EventHandler(this.mantenimientoDeClientesToolStripMenuItem_Click);
            // 
            // comprasToolStripMenuItem
            // 
            this.comprasToolStripMenuItem.Name = "comprasToolStripMenuItem";
            this.comprasToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.comprasToolStripMenuItem.Text = "Compras";
            this.comprasToolStripMenuItem.Click += new System.EventHandler(this.comprasToolStripMenuItem_Click);
            // 
            // frmMenuPrincipalToolStripMenuItem
            // 
            this.frmMenuPrincipalToolStripMenuItem.Name = "frmMenuPrincipalToolStripMenuItem";
            this.frmMenuPrincipalToolStripMenuItem.Size = new System.Drawing.Size(174, 20);
            this.frmMenuPrincipalToolStripMenuItem.Text = "Mantenimiento de productos";
            this.frmMenuPrincipalToolStripMenuItem.Click += new System.EventHandler(this.frmMenuPrincipalToolStripMenuItem_Click);
            // 
            // FrmMenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMenuPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mantenimientoDeProvedoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entradaDeProductosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mantenimientoDeClientesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem comprasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frmMenuPrincipalToolStripMenuItem;
    }
}