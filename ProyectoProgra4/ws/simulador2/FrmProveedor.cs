using System;
using System.Drawing;
using System.Windows.Forms;
// Se utiliza el espacio de nombres exacto de tu proyecto y referencia
using simulador2.RefWsAlmacen;

namespace simulador2
{
    public partial class FrmProveedor : Form
    {
        // 1. Declaración de las variables para los 7 campos del proveedor
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
            // Inicialización base del formulario
            InitializeComponent();

            // Llamada al método que dibuja los controles
            ConstruirInterfaz();
        }

        // Método encargado de crear y posicionar todo visualmente sin el diseñador
        private void ConstruirInterfaz()
        {
            // Configuración general de la ventana
            this.Text = "Mantenimiento de Proveedor";
            this.Size = new Size(480, 430);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Instanciar RadioButtons (1 carácter en la trama)
            rbIngreso = new RadioButton { Text = "Ingreso", Location = new Point(20, 20), Checked = true };
            rbModificacion = new RadioButton { Text = "Modificación", Location = new Point(120, 20) };

            // Instanciar Labels y TextBox para Cédula (Max 10 caracteres)
            Label lblCedula = new Label { Text = "Cédula Jurídica:", Location = new Point(20, 60), AutoSize = true };
            txtCedula = new TextBox { Location = new Point(170, 56), Width = 250, MaxLength = 10 };

            // Instanciar Labels y TextBox para Nombre Empresa (Max 100 caracteres)
            Label lblEmpresa = new Label { Text = "Nombre Empresa:", Location = new Point(20, 100), AutoSize = true };
            txtNombreEmpresa = new TextBox { Location = new Point(170, 96), Width = 250, MaxLength = 100 };

            // Instanciar Labels y TextBox para Nombre Contacto (Max 75 caracteres)
            Label lblContacto = new Label { Text = "Nombre Contacto:", Location = new Point(20, 140), AutoSize = true };
            txtNombreContacto = new TextBox { Location = new Point(170, 136), Width = 250, MaxLength = 75 };

            // Instanciar Labels y TextBox para Teléfono (Max 8 caracteres)
            Label lblTelefono = new Label { Text = "Teléfono:", Location = new Point(20, 180), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(170, 176), Width = 250, MaxLength = 8 };

            // Instanciar Labels y TextBox para Correo (Max 75 caracteres)
            Label lblCorreo = new Label { Text = "Correo:", Location = new Point(20, 220), AutoSize = true };
            txtCorreo = new TextBox { Location = new Point(170, 216), Width = 250, MaxLength = 75 };

            // Instanciar Labels y TextBox para Estado (Max 1 carácter, ej: 'A' o 'I')
            Label lblEstado = new Label { Text = "Estado (1 letra):", Location = new Point(20, 260), AutoSize = true };
            txtEstado = new TextBox { Location = new Point(170, 256), Width = 50, MaxLength = 1 };

            // Instanciar y configurar el Botón de Guardar
            btnGuardar = new Button { Text = "Guardar", Location = new Point(170, 310), Width = 100, Height = 35 };

            // Suscribir el botón a su evento de lógica
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            // Agregar todos los controles a la colección de la ventana para hacerlos visibles
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

        // Lógica de consumo del Web Service al presionar el botón
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificación de campos clave obligatorios
                if (string.IsNullOrWhiteSpace(txtCedula.Text) || string.IsNullOrWhiteSpace(txtNombreEmpresa.Text))
                {
                    MessageBox.Show("La cédula y el nombre de la empresa son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1. Extracción y limpieza de los textos introducidos
                string tipoTransaccion = rbIngreso.Checked ? "1" : "2";
                string cedula = txtCedula.Text.Trim();
                string empresa = txtNombreEmpresa.Text.Trim();
                string contacto = txtNombreContacto.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string estado = txtEstado.Text.Trim();

                // 2. Instancia del cliente SOAP y ejecución de la operación del proveedor
                ServicioAlmacenSoapClient clienteWs = new ServicioAlmacenSoapClient();
                Respuesta respuesta = clienteWs.MantenimientoProveedor(tipoTransaccion, cedula, empresa, contacto, telefono, correo, estado);

                // 3. Lectura del objeto de respuesta estandarizado
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

        // Resetea el formulario a su estado original después de un registro exitoso
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