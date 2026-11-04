using System;
using System.Drawing;
using System.Windows.Forms;

namespace simulador2
{
    public partial class FrmCompraVerificador : Form
    {
        private TextBox txtNumeroCompra;
        private TextBox txtIdCliente;
        private DateTimePicker dtpFechaCompra;
        private NumericUpDown numTotalCompra;
        private TextBox txtNumeroTarjeta;
        private TextBox txtFechaVencimiento;
        private TextBox txtCodigoVerificacion;
        private TextBox txtCodigoProducto;
        private NumericUpDown numCantidad;
        private Button btnAgregarProducto;
        private DataGridView dgvProductos;
        private Button btnRegistrar;

        public FrmCompraVerificador()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.BackColor = SystemColors.Control;
            this.Text = "Pago de Compra - Verificador";
            this.Size = new Size(620, 780);
            this.StartPosition = FormStartPosition.CenterScreen;

            // CABECERA
            Label lblNumCompra = new Label { Text = "* No. Compra (Max 10):", Location = new Point(20, 20), AutoSize = true };
            txtNumeroCompra = new TextBox { Location = new Point(20, 45), Width = 260, TabIndex = 0 };

            Label lblCliente = new Label { Text = "* ID Cliente:", Location = new Point(300, 20), AutoSize = true };
            txtIdCliente = new TextBox { Location = new Point(300, 45), Width = 270, TabIndex = 1 };

            Label lblFecha = new Label { Text = "Fecha Compra:", Location = new Point(20, 85), AutoSize = true };
            dtpFechaCompra = new DateTimePicker { Location = new Point(20, 110), Width = 260, Format = DateTimePickerFormat.Short, TabIndex = 2 };

            Label lblTotal = new Label { Text = "Total Compra:", Location = new Point(300, 85), AutoSize = true };
            numTotalCompra = new NumericUpDown { Location = new Point(300, 110), Width = 270, DecimalPlaces = 2, Maximum = 99999999m, TabIndex = 3 };

            // DATOS BANCARIOS (CIFRADOS)
            GroupBox gbBanco = new GroupBox { Text = "Datos de Tarjeta (Se enviarán cifrados)", Location = new Point(20, 160), Size = new Size(550, 100) };
            gbBanco.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTarjeta = new Label { Text = "* Tarjeta (Max 16):", Location = new Point(15, 30), AutoSize = true };
            txtNumeroTarjeta = new TextBox { Location = new Point(15, 55), Width = 250, MaxLength = 16, TabIndex = 4 };

            Label lblVence = new Label { Text = "* Vence (MM/AA):", Location = new Point(280, 30), AutoSize = true };
            txtFechaVencimiento = new TextBox { Location = new Point(280, 55), Width = 150, MaxLength = 5, TabIndex = 5 };

            Label lblCvv = new Label { Text = "* CVV:", Location = new Point(450, 30), AutoSize = true };
            txtCodigoVerificacion = new TextBox { Location = new Point(450, 55), Width = 80, MaxLength = 4, TabIndex = 6 };

            gbBanco.Controls.Add(lblTarjeta); gbBanco.Controls.Add(txtNumeroTarjeta);
            gbBanco.Controls.Add(lblVence); gbBanco.Controls.Add(txtFechaVencimiento);
            gbBanco.Controls.Add(lblCvv); gbBanco.Controls.Add(txtCodigoVerificacion);

            // DETALLE PRODUCTOS
            GroupBox gbDetalle = new GroupBox { Text = "Agregar Producto a la Lista", Location = new Point(20, 280), Size = new Size(550, 100) };
            gbDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblCodProd = new Label { Text = "* Código (Max 10):", Location = new Point(15, 30), AutoSize = true };
            txtCodigoProducto = new TextBox { Location = new Point(15, 55), Width = 200, TabIndex = 7 };

            Label lblCant = new Label { Text = "Cant:", Location = new Point(230, 30), AutoSize = true };
            numCantidad = new NumericUpDown { Location = new Point(230, 55), Width = 100, Minimum = 1, Maximum = 9999, TabIndex = 8 };

            btnAgregarProducto = new Button { Text = "Agregar", Location = new Point(380, 52), Width = 150, Height = 30, UseVisualStyleBackColor = true, TabIndex = 9 };
            btnAgregarProducto.Click += new EventHandler(btnAgregarProducto_Click);

            gbDetalle.Controls.Add(lblCodProd); gbDetalle.Controls.Add(txtCodigoProducto);
            gbDetalle.Controls.Add(lblCant); gbDetalle.Controls.Add(numCantidad);
            gbDetalle.Controls.Add(btnAgregarProducto);

            // TABLA Y ENVÍO
            dgvProductos = new DataGridView
            {
                Location = new Point(20, 400),
                Size = new Size(550, 230),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                TabIndex = 10,
                // Mejoras visuales
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            dgvProductos.Columns.Add("Codigo", "Código Producto");
            dgvProductos.Columns.Add("Cantidad", "Cantidad");

            btnRegistrar = new Button { Text = "Registrar y Enviar", Location = new Point(370, 650), Width = 200, Height = 45, UseVisualStyleBackColor = true, TabIndex = 11 };
            btnRegistrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRegistrar.Click += new EventHandler(btnRegistrar_Click);

            this.AcceptButton = btnRegistrar;

            this.Controls.Add(lblNumCompra); this.Controls.Add(txtNumeroCompra);
            this.Controls.Add(lblCliente); this.Controls.Add(txtIdCliente);
            this.Controls.Add(lblFecha); this.Controls.Add(dtpFechaCompra);
            this.Controls.Add(lblTotal); this.Controls.Add(numTotalCompra);
            this.Controls.Add(gbBanco);
            this.Controls.Add(gbDetalle);
            this.Controls.Add(dgvProductos);
            this.Controls.Add(btnRegistrar);
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigoProducto.Text)) return;
            dgvProductos.Rows.Add(txtCodigoProducto.Text.Trim(), numCantidad.Value.ToString());
            txtCodigoProducto.Clear();
            numCantidad.Value = 1;
            txtCodigoProducto.Focus();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductos.Rows.Count == 0 || string.IsNullOrWhiteSpace(txtNumeroCompra.Text) || string.IsNullOrWhiteSpace(txtIdCliente.Text))
                {
                    MessageBox.Show("Asegúrese de llenar los campos obligatorios (*) y agregar al menos un producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string numeroCompra = txtNumeroCompra.Text.Trim();
                string idCliente = txtIdCliente.Text.Trim();
                string fechaCompra = dtpFechaCompra.Value.ToString("yyyyMMdd");
                string totalCompra = numTotalCompra.Value.ToString("0.00");

                this.Cursor = Cursors.WaitCursor;

                string tarjetaCifrada = CifradoAes.Cifrar(txtNumeroTarjeta.Text.Trim());
                string vencimientoCifrado = CifradoAes.Cifrar(txtFechaVencimiento.Text.Trim());
                string cvvCifrado = CifradoAes.Cifrar(txtCodigoVerificacion.Text.Trim());

                MessageBox.Show($"Datos bancarios listos para SOAP:\n\nTarjeta: {tarjetaCifrada}\nCVV: {cvvCifrado}", "Cifrado Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}