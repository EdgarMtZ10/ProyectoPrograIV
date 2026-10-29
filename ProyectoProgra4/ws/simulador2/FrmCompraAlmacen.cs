using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
// Se utiliza el espacio de nombres exacto de tu proyecto y referencia
using simulador2.RefWsAlmacen;

namespace simulador2
{
    public partial class FrmCompraAlmacen : Form
    {
        // Controles de cabecera (transacción)
        private TextBox txtNumeroIngreso;
        private DateTimePicker dtpFechaCompra;
        private TextBox txtCedula;

        // Controles de detalle (ingreso de producto a la lista)
        private TextBox txtNumeroProducto;
        private NumericUpDown numCantidad;
        private Button btnAgregarProducto;

        // Controles de visualización y envío
        private DataGridView dgvProductos;
        private Button btnGuardar;

        public FrmCompraAlmacen()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Registro de Compra - Almacén";
            this.Size = new Size(550, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- SECCIÓN CABECERA ---
            Label lblIngreso = new Label { Text = "Número de Ingreso:", Location = new Point(20, 20), AutoSize = true };
            txtNumeroIngreso = new TextBox { Location = new Point(150, 16), Width = 150, MaxLength = 10 };

            Label lblFecha = new Label { Text = "Fecha Compra:", Location = new Point(20, 60), AutoSize = true };
            // DateTimePicker garantiza que el usuario seleccione una fecha válida, evitando errores de formato.
            dtpFechaCompra = new DateTimePicker { Location = new Point(150, 56), Width = 150, Format = DateTimePickerFormat.Short };

            Label lblCedula = new Label { Text = "Cédula Jurídica:", Location = new Point(20, 100), AutoSize = true };
            txtCedula = new TextBox { Location = new Point(150, 96), Width = 150, MaxLength = 10 };

            // --- SECCIÓN DETALLE (PRODUCTOS) ---
            GroupBox gbDetalle = new GroupBox { Text = "Agregar Producto", Location = new Point(20, 140), Size = new Size(490, 80) };

            Label lblNoProd = new Label { Text = "No. Producto:", Location = new Point(15, 30), AutoSize = true };
            txtNumeroProducto = new TextBox { Location = new Point(100, 26), Width = 100, MaxLength = 10 };

            Label lblCant = new Label { Text = "Cantidad:", Location = new Point(220, 30), AutoSize = true };
            numCantidad = new NumericUpDown { Location = new Point(280, 26), Width = 80, Minimum = 1, Maximum = 9999999 };

            btnAgregarProducto = new Button { Text = "Agregar", Location = new Point(380, 24), Width = 90, Height = 25 };
            btnAgregarProducto.Click += new EventHandler(btnAgregarProducto_Click);

            gbDetalle.Controls.Add(lblNoProd);
            gbDetalle.Controls.Add(txtNumeroProducto);
            gbDetalle.Controls.Add(lblCant);
            gbDetalle.Controls.Add(numCantidad);
            gbDetalle.Controls.Add(btnAgregarProducto);

            // --- SECCIÓN GRILLA Y ENVÍO ---
            dgvProductos = new DataGridView
            {
                Location = new Point(20, 240),
                Size = new Size(490, 200),
                AllowUserToAddRows = false, // Evita la fila vacía al final
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            // Definición manual de las columnas de la grilla
            dgvProductos.Columns.Add("NoProducto", "Número de Producto");
            dgvProductos.Columns.Add("Cantidad", "Cantidad");
            dgvProductos.Columns[0].Width = 250;
            dgvProductos.Columns[1].Width = 150;

            btnGuardar = new Button { Text = "Registrar Compra", Location = new Point(190, 460), Width = 150, Height = 35 };
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            // Agregar a la ventana
            this.Controls.Add(lblIngreso);
            this.Controls.Add(txtNumeroIngreso);
            this.Controls.Add(lblFecha);
            this.Controls.Add(dtpFechaCompra);
            this.Controls.Add(lblCedula);
            this.Controls.Add(txtCedula);
            this.Controls.Add(gbDetalle);
            this.Controls.Add(dgvProductos);
            this.Controls.Add(btnGuardar);
        }

        // Lógica para poblar la grilla temporalmente antes de enviar
        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumeroProducto.Text))
            {
                MessageBox.Show("Ingrese un número de producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Agrega una nueva fila al DataGridView con los valores introducidos
            dgvProductos.Rows.Add(txtNumeroProducto.Text.Trim(), numCantidad.Value.ToString());

            // Limpia las cajas de texto de detalle para el siguiente producto
            txtNumeroProducto.Clear();
            numCantidad.Value = 1;
            txtNumeroProducto.Focus();
        }

        // Lógica principal: armado de la lista dinámica y consumo del ASMX
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNumeroIngreso.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    MessageBox.Show("El número de ingreso y la cédula son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvProductos.Rows.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un producto a la lista.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string numeroIngreso = txtNumeroIngreso.Text.Trim();
                // El parser de Java espera la fecha en formato YYYYMMDD (8 caracteres), por lo que se formatea el Value del DateTimePicker
                string fechaCompra = dtpFechaCompra.Value.ToString("yyyyMMdd");
                string cedula = txtCedula.Text.Trim();

                // Las referencias de servicio en Visual Studio convierten List<T> en arrays (T[]) por defecto.
                // Inicializamos el array con el tamaño exacto de filas en la grilla.
                ProductoCompra[] listaProductos = new ProductoCompra[dgvProductos.Rows.Count];

                // Se itera sobre las filas visuales para construir los objetos que viajarán por SOAP
                // Se itera sobre las filas visuales para construir los objetos que viajarán por SOAP
                for (int i = 0; i < dgvProductos.Rows.Count; i++)
                {
                    listaProductos[i] = new ProductoCompra
                    {
                        NoProducto = dgvProductos.Rows[i].Cells["NoProducto"].Value.ToString(),
                        // Se envía como string respetando tu defensa contra errores HTTP 500
                        Cantidad = dgvProductos.Rows[i].Cells["Cantidad"].Value.ToString()
                    };
                }

                // Invocación del Web Service
                ServicioAlmacenSoapClient clienteWs = new ServicioAlmacenSoapClient();
                Respuesta respuesta = clienteWs.RegistrarCompra(numeroIngreso, fechaCompra, cedula, listaProductos);

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
            txtNumeroIngreso.Clear();
            dtpFechaCompra.Value = DateTime.Now;
            txtCedula.Clear();
            txtNumeroProducto.Clear();
            numCantidad.Value = 1;
            dgvProductos.Rows.Clear(); // Vacía la grilla
            txtNumeroIngreso.Focus();
        }
    }
}