using simulador2.RefWsAlmacen;
// Cambia esto por el nombre real de tu referencia y proyecto
using simulador2.RefWsAlmacen;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador2
{
    public partial class FrmProducto : Form
    {
        // 1. Declaración de las variables de los controles a nivel de clase
        private RadioButton rbIngreso;
        private RadioButton rbModificacion;
        private TextBox txtNumeroProducto;
        private TextBox txtNombre;
        private NumericUpDown numPrecio;
        private Button btnGuardar;

        public FrmProducto()
        {
            // Este método nativo inicializa lo básico del form, déjalo ahí.
            InitializeComponent();

            // Llamamos a nuestro propio método para pintar la interfaz
            ConstruirInterfaz();
        }

        // Método encargado de crear y posicionar todo visualmente
        private void ConstruirInterfaz()
        {
            // Configuración general de la ventana
            this.Text = "Mantenimiento de Producto";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Instanciar y configurar RadioButtons (Tipo de transacción)
            rbIngreso = new RadioButton { Text = "Ingreso", Location = new Point(20, 20), Checked = true };
            rbModificacion = new RadioButton { Text = "Modificación", Location = new Point(120, 20) };

            // Instanciar Labels y TextBox para Número de Producto
            Label lblNumero = new Label { Text = "Número de Producto:", Location = new Point(20, 60), AutoSize = true };
            txtNumeroProducto = new TextBox { Location = new Point(150, 56), Width = 200, MaxLength = 10 };

            // Instanciar Labels y TextBox para Nombre del Producto
            Label lblNombre = new Label { Text = "Nombre del Producto:", Location = new Point(20, 100), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(150, 96), Width = 200, MaxLength = 90 };

            // Instanciar Labels y NumericUpDown para el Precio
            Label lblPrecio = new Label { Text = "Precio:", Location = new Point(20, 140), AutoSize = true };
            numPrecio = new NumericUpDown { Location = new Point(150, 136), Width = 200, DecimalPlaces = 2, Maximum = 999999.99m };

            // Instanciar y configurar el Botón de Guardar
            btnGuardar = new Button { Text = "Guardar", Location = new Point(150, 190), Width = 100, Height = 35 };

            // Suscribir el botón al evento Click (esto enlaza el diseño con la lógica)
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            // 3. Agregar todos los controles a la pantalla para que sean visibles
            this.Controls.Add(rbIngreso);
            this.Controls.Add(rbModificacion);
            this.Controls.Add(lblNumero);
            this.Controls.Add(txtNumeroProducto);
            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);
            this.Controls.Add(lblPrecio);
            this.Controls.Add(numPrecio);
            this.Controls.Add(btnGuardar);
        }

        // Lógica del botón (exactamente la misma que usarías con el diseñador visual)
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNumeroProducto.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El número de producto y el nombre son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string tipoTransaccion = rbIngreso.Checked ? "1" : "2";
                string numeroProducto = txtNumeroProducto.Text.Trim();
                string nombreProducto = txtNombre.Text.Trim();
                decimal precio = numPrecio.Value;

                // Consumo del Web Service
                ServicioAlmacenSoapClient clienteWs = new ServicioAlmacenSoapClient();
                Respuesta respuesta = clienteWs.MantenimientoProducto(tipoTransaccion, numeroProducto, nombreProducto, precio);

                if (respuesta.Resultado)
                {
                    MessageBox.Show(respuesta.Mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rbIngreso.Checked = true;
                    txtNumeroProducto.Clear();
                    txtNombre.Clear();
                    numPrecio.Value = 0;
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
    }
}