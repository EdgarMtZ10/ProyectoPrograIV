using simulador2.RefWsAlmacen;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador2
{
    public partial class FrmProducto : Form
    {
        private RadioButton rbIngreso;
        private RadioButton rbModificacion;
        private TextBox txtNumeroProducto;
        private TextBox txtNombre;
        private NumericUpDown numPrecio;
        private Button btnGuardar;

        public FrmProducto()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            // Reglas de UI: Segoe UI, sin excesos, colores de sistema
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.BackColor = SystemColors.Control;
            this.Text = "Mantenimiento de Producto";
            this.Size = new Size(450, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            rbIngreso = new RadioButton { Text = "Ingreso", Location = new Point(20, 20), Checked = true, AutoSize = true };
            rbModificacion = new RadioButton { Text = "Modificación", Location = new Point(120, 20), AutoSize = true };

            // Etiquetas con límites y ejemplos, controles apilados para mantener el orden y legibilidad
            Label lblNumero = new Label { Text = "Número de Producto (Max 10, Ej: 1001):", Location = new Point(20, 60), AutoSize = true };
            txtNumeroProducto = new TextBox { Location = new Point(20, 85), Width = 390, MaxLength = 10 };

            Label lblNombre = new Label { Text = "Nombre del Producto:", Location = new Point(20, 130), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(20, 155), Width = 390, MaxLength = 90 };

            Label lblPrecio = new Label { Text = "Precio:", Location = new Point(20, 200), AutoSize = true };
            numPrecio = new NumericUpDown { Location = new Point(20, 225), Width = 390, DecimalPlaces = 2, Maximum = 999999.99m };

            btnGuardar = new Button { Text = "Guardar", Location = new Point(175, 300), Width = 100, Height = 35, UseVisualStyleBackColor = true };
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

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