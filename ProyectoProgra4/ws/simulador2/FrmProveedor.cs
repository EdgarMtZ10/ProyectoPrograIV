using System;
using System.Drawing;
using System.Windows.Forms;
using simulador2.RefWsAlmacen;

namespace simulador2
{
    public partial class FrmProveedor : Form
    {
        // Declaración de las variables para los 7 campos del proveedor
        private RadioButton rbIngreso;
        private RadioButton rbModificacion;
        private TextBox txtCedula;
        private TextBox txtNombreEmpresa;
        private TextBox txtNombreContacto;
        private TextBox txtTelefono;
        private TextBox txtCorreo;
        private TextBox txtEstado;
        private Button btnGuardar;

        public FrmProveedor()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            // Configuración general de la ventana con diseño limpio y Segoe UI
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.BackColor = SystemColors.Control;
            this.Text = "Mantenimiento de Proveedor";
            this.Size = new Size(450, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Instanciar RadioButtons
            rbIngreso = new RadioButton { Text = "Ingreso", Location = new Point(20, 20), Checked = true, AutoSize = true };
            rbModificacion = new RadioButton { Text = "Modificación", Location = new Point(120, 20), AutoSize = true };

            // Etiquetas con límites y ejemplos, controles apilados verticalmente
            Label lblCedula = new Label { Text = "Cédula Jurídica (Max 10, Ej: 3101123456):", Location = new Point(20, 60), AutoSize = true };
            txtCedula = new TextBox { Location = new Point(20, 85), Width = 390, MaxLength = 10 };

            Label lblEmpresa = new Label { Text = "Nombre Empresa (Ej: Nova Online):", Location = new Point(20, 130), AutoSize = true };
            txtNombreEmpresa = new TextBox { Location = new Point(20, 155), Width = 390, MaxLength = 100 };

            Label lblContacto = new Label { Text = "Nombre Contacto (Ej: Juan Pérez):", Location = new Point(20, 200), AutoSize = true };
            txtNombreContacto = new TextBox { Location = new Point(20, 225), Width = 390, MaxLength = 75 };

            Label lblTelefono = new Label { Text = "Teléfono (Ej: 88889999):", Location = new Point(20, 270), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(20, 295), Width = 390, MaxLength = 8 };

            Label lblCorreo = new Label { Text = "Correo (Ej: contacto@empresa.com):", Location = new Point(20, 340), AutoSize = true };
            txtCorreo = new TextBox { Location = new Point(20, 365), Width = 390, MaxLength = 75 };

            Label lblEstado = new Label { Text = "Estado (1 para Activo, 2 para Inactivo):", Location = new Point(20, 410), AutoSize = true };
            txtEstado = new TextBox { Location = new Point(20, 435), Width = 390, MaxLength = 1 };

            btnGuardar = new Button { Text = "Guardar", Location = new Point(175, 485), Width = 100, Height = 35, UseVisualStyleBackColor = true };
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            // Agregar controles a la ventana
            this.Controls.Add(rbIngreso);
            this.Controls.Add(rbModificacion);
            this.Controls.Add(lblCedula);
            this.Controls.Add(txtCedula);
            this.Controls.Add(lblEmpresa);
            this.Controls.Add(txtNombreEmpresa);
            this.Controls.Add(lblContacto);
            this.Controls.Add(txtNombreContacto);
            this.Controls.Add(lblTelefono);
            this.Controls.Add(txtTelefono);
            this.Controls.Add(lblCorreo);
            this.Controls.Add(txtCorreo);
            this.Controls.Add(lblEstado);
            this.Controls.Add(txtEstado);
            this.Controls.Add(btnGuardar);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedula.Text) || string.IsNullOrWhiteSpace(txtNombreEmpresa.Text))
                {
                    MessageBox.Show("La cédula y el nombre de la empresa son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // El Almacén enruta por el primer carácter (3 = ingreso proveedor, 4 = modificación proveedor)
                string tipoTransaccion = rbIngreso.Checked ? "3" : "4";
                string cedula = txtCedula.Text.Trim();
                string empresa = txtNombreEmpresa.Text.Trim();
                string contacto = txtNombreContacto.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string estado = txtEstado.Text.Trim();

                ServicioAlmacenSoapClient clienteWs = new ServicioAlmacenSoapClient();
                Respuesta respuesta = clienteWs.MantenimientoProveedor(tipoTransaccion, cedula, empresa, contacto, telefono, correo, estado);

                if (respuesta.Resultado)
                {
                    MessageBox.Show(respuesta.Mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show(respuesta.Mensaje, "Aviso del Servidor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            rbIngreso.Checked = true;
            txtCedula.Clear();
            txtNombreEmpresa.Clear();
            txtNombreContacto.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtEstado.Clear();
            txtCedula.Focus();
        }
    }
}