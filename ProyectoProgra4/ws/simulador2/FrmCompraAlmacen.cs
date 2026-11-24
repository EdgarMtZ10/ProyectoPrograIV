using System;
using System.Drawing;
using System.Windows.Forms;
using simulador2.RefWsAlmacen;

namespace simulador2
{
    public partial class FrmCompraAlmacen : Form
    {
        private TextBox txtNumeroIngreso;
        private DateTimePicker dtpFechaCompra;
        private TextBox txtCedula;
        private TextBox txtNumeroProducto;
        private NumericUpDown numCantidad;
        private Button btnAgregarProducto;
        private DataGridView dgvProductos;
        private Button btnGuardar;

        public FrmCompraAlmacen()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.BackColor = SystemColors.Control;
            this.Text = "Registro de Compra - Almacén";
            this.Size = new Size(580, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            // CABECERA
            // CORRECCIÓN 1: Cambio de etiqueta para evitar letras en el BIGINT
            Label lblIngreso = new Label { Text = "* No. Ingreso (Solo números, Ej: 1001):", Location = new Point(20, 20), AutoSize = true };
            txtNumeroIngreso = new TextBox { Location = new Point(20, 45), Width = 160, MaxLength = 10, TabIndex = 0 };

            Label lblFecha = new Label { Text = "Fecha Compra:", Location = new Point(200, 20), AutoSize = true };
            dtpFechaCompra = new DateTimePicker { Location = new Point(200, 45), Width = 160, Format = DateTimePickerFormat.Short, TabIndex = 1 };

            // CORRECCIÓN 2: Bloquear fechas futuras para no violar la regla GETDATE() de SQL Server
            dtpFechaCompra.MaxDate = DateTime.Now;

            Label lblCedula = new Label { Text = "* Cédula Jurídica (Max 10, Ej: 3101123456):", Location = new Point(20, 90), AutoSize = true };
            txtCedula = new TextBox { Location = new Point(20, 115), Width = 340, MaxLength = 10, TabIndex = 2 };

            // DETALLE
            GroupBox gbDetalle = new GroupBox { Text = "Agregar Producto a la Lista", Location = new Point(20, 170), Size = new Size(520, 100) };
            gbDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblNoProd = new Label { Text = "* No. Producto:", Location = new Point(15, 30), AutoSize = true };
            txtNumeroProducto = new TextBox { Location = new Point(15, 55), Width = 200, MaxLength = 10, TabIndex = 3 };

            Label lblCant = new Label { Text = "Cantidad:", Location = new Point(230, 30), AutoSize = true };
            numCantidad = new NumericUpDown { Location = new Point(230, 55), Width = 120, Minimum = 1, Maximum = 9999999, TabIndex = 4 };

            btnAgregarProducto = new Button { Text = "Agregar", Location = new Point(380, 52), Width = 100, Height = 30, UseVisualStyleBackColor = true, TabIndex = 5 };
            btnAgregarProducto.Click += new EventHandler(btnAgregarProducto_Click);

            gbDetalle.Controls.Add(lblNoProd);
            gbDetalle.Controls.Add(txtNumeroProducto);
            gbDetalle.Controls.Add(lblCant);
            gbDetalle.Controls.Add(numCantidad);
            gbDetalle.Controls.Add(btnAgregarProducto);

            // TABLA Y ENVÍO
            dgvProductos = new DataGridView
            {
                Location = new Point(20, 290),
                Size = new Size(520, 250),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                TabIndex = 6,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            dgvProductos.Columns.Add("NoProducto", "Número de Producto");
            dgvProductos.Columns.Add("Cantidad", "Cantidad");

            btnGuardar = new Button { Text = "Registrar Compra", Location = new Point(360, 570), Width = 180, Height = 40, UseVisualStyleBackColor = true, TabIndex = 7 };
            btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            this.AcceptButton = btnGuardar;

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

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            string codigoProducto = txtNumeroProducto.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigoProducto))
            {
                MessageBox.Show("Ingrese un número de producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // CORRECCIÓN 3: Prevenir que el usuario escriba letras en el producto (Regla: '%[^0-9]%')
            if (!long.TryParse(codigoProducto, out _))
            {
                MessageBox.Show("El código de producto debe ser puramente numérico (Ej: 3030303030).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvProductos.Rows.Add(codigoProducto, numCantidad.Value.ToString());
            txtNumeroProducto.Clear();
            numCantidad.Value = 1;
            txtNumeroProducto.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNumeroIngreso.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    MessageBox.Show("El número de ingreso y la cédula son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CORRECCIÓN 4: Prevenir que la base de datos estalle al intentar meter letras en un BIGINT
                if (!long.TryParse(txtNumeroIngreso.Text.Trim(), out _))
                {
                    MessageBox.Show("El número de ingreso de la compra debe ser puramente numérico.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvProductos.Rows.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un producto a la lista.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string numeroIngreso = txtNumeroIngreso.Text.Trim();
                string fechaCompra = dtpFechaCompra.Value.ToString("yyyyMMdd");
                string cedula = txtCedula.Text.Trim();

                ProductoCompra[] listaProductos = new ProductoCompra[dgvProductos.Rows.Count];
                for (int i = 0; i < dgvProductos.Rows.Count; i++)
                {
                    listaProductos[i] = new ProductoCompra
                    {
                        NoProducto = dgvProductos.Rows[i].Cells["NoProducto"].Value.ToString(),
                        Cantidad = dgvProductos.Rows[i].Cells["Cantidad"].Value.ToString()
                    };
                }

                this.Cursor = Cursors.WaitCursor;
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
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void LimpiarCampos()
        {
            txtNumeroIngreso.Clear();
            dtpFechaCompra.Value = DateTime.Now;
            txtCedula.Clear();
            txtNumeroProducto.Clear();
            numCantidad.Value = 1;
            dgvProductos.Rows.Clear();
            txtNumeroIngreso.Focus();
        }
    }
}