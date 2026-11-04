using System;
using System.Drawing;
using System.Windows.Forms;
using simulador2.RefWsAutenticacion;

namespace simulador2
{
    [System.ComponentModel.DesignerCategory("Code")]
    public partial class FrmAutenticacion : Form
    {
        private TabControl tabControl;
        private TextBox txtRespuesta;

        // Controles Autenticar
        private TextBox txtAuthUsuario;
        private TextBox txtAuthPass;
        private ComboBox cmbAuthTipo;
        private Button btnAutenticar;

        // Controles Crear
        private TextBox txtCrearId;
        private TextBox txtCrearNombre;
        private TextBox txtCrearApe1;
        private TextBox txtCrearApe2;
        private TextBox txtCrearCorreo;
        private TextBox txtCrearUsuario;
        private TextBox txtCrearPass;
        private ComboBox cmbCrearEstado;
        private ComboBox cmbCrearTipo;
        private Button btnCrear;

        // Controles Modificar
        private TextBox txtModId;
        private TextBox txtModNombre;
        private TextBox txtModApe1;
        private TextBox txtModApe2;
        private TextBox txtModCorreo;
        private TextBox txtModUsuario;
        private TextBox txtModPass;
        private Button btnModificar;

        // Controles Cambiar Estado
        private TextBox txtEstId;
        private ComboBox cmbEstEstado;
        private Button btnEstado;

        public FrmAutenticacion()
        {
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Mantenimiento de Usuarios y Autenticación";
            this.Size = new Size(500, 520);
            this.StartPosition = FormStartPosition.CenterScreen;

            txtRespuesta = new TextBox();
            txtRespuesta.Multiline = true;
            txtRespuesta.ReadOnly = true;
            txtRespuesta.Height = 80;
            txtRespuesta.Dock = DockStyle.Bottom;

            Label lblRespuesta = new Label();
            lblRespuesta.Text = "Respuesta del servicio:";
            lblRespuesta.Dock = DockStyle.Bottom;
            lblRespuesta.Padding = new Padding(0, 5, 0, 2);

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            TabPage tabAuth = new TabPage("Autenticar");
            TabPage tabCrear = new TabPage("Crear usuario");
            TabPage tabMod = new TabPage("Modificar usuario");
            TabPage tabEst = new TabPage("Cambiar estado");

            // --- Pestaña: Autenticar ---
            CrearCampoTexto(tabAuth, "Usuario:", 20, out txtAuthUsuario);
            CrearCampoTexto(tabAuth, "Contraseña:", 60, out txtAuthPass, true);
            CrearCombo(tabAuth, "Tipo:", 100, out cmbAuthTipo, new string[] { "1 - Empleado o administrador", "2 - Cliente" });

            btnAutenticar = new Button { Text = "Autenticar", Location = new Point(150, 150), Width = 100, Height = 30 };
            btnAutenticar.Click += new EventHandler(btnAutenticar_Click);
            tabAuth.Controls.Add(btnAutenticar);

            // --- Pestaña: Crear usuario ---
            CrearCampoTexto(tabCrear, "Identificación:", 20, out txtCrearId);
            CrearCampoTexto(tabCrear, "Nombre:", 50, out txtCrearNombre);
            CrearCampoTexto(tabCrear, "Primer Apellido:", 80, out txtCrearApe1);
            CrearCampoTexto(tabCrear, "Segundo Apel.:", 110, out txtCrearApe2);
            CrearCampoTexto(tabCrear, "Correo:", 140, out txtCrearCorreo);
            CrearCampoTexto(tabCrear, "Usuario:", 170, out txtCrearUsuario);
            CrearCampoTexto(tabCrear, "Contraseña:", 200, out txtCrearPass, true);
            CrearCombo(tabCrear, "Estado:", 230, out cmbCrearEstado, new string[] { "1 - Activo" });
            CrearCombo(tabCrear, "Tipo:", 260, out cmbCrearTipo, new string[] { "1 - Empleado o administrador", "2 - Cliente" });

            btnCrear = new Button { Text = "Crear", Location = new Point(150, 310), Width = 100, Height = 30 };
            btnCrear.Click += new EventHandler(btnCrear_Click);
            tabCrear.Controls.Add(btnCrear);

            // --- Pestaña: Modificar usuario ---
            CrearCampoTexto(tabMod, "Identificación:", 20, out txtModId);
            CrearCampoTexto(tabMod, "Nombre:", 50, out txtModNombre);
            CrearCampoTexto(tabMod, "Primer Apellido:", 80, out txtModApe1);
            CrearCampoTexto(tabMod, "Segundo Apel.:", 110, out txtModApe2);
            CrearCampoTexto(tabMod, "Correo:", 140, out txtModCorreo);
            CrearCampoTexto(tabMod, "Usuario:", 170, out txtModUsuario);
            CrearCampoTexto(tabMod, "Contraseña:", 200, out txtModPass, true);

            btnModificar = new Button { Text = "Modificar", Location = new Point(150, 250), Width = 100, Height = 30 };
            btnModificar.Click += new EventHandler(btnModificar_Click);
            tabMod.Controls.Add(btnModificar);

            // --- Pestaña: Cambiar estado ---
            CrearCampoTexto(tabEst, "Identificación:", 20, out txtEstId);
            CrearCombo(tabEst, "Estado:", 60, out cmbEstEstado, new string[] { "1 - Activo", "2 - Inactivo" });

            btnEstado = new Button { Text = "Cambiar", Location = new Point(150, 110), Width = 100, Height = 30 };
            btnEstado.Click += new EventHandler(btnEstado_Click);
            tabEst.Controls.Add(btnEstado);

            tabControl.TabPages.Add(tabAuth);
            tabControl.TabPages.Add(tabCrear);
            tabControl.TabPages.Add(tabMod);
            tabControl.TabPages.Add(tabEst);

            // EVENTO AGREGADO: Limpiar la respuesta al cambiar de pestaña
            tabControl.SelectedIndexChanged += (s, e) => txtRespuesta.Clear();

            this.Controls.Add(tabControl);
            this.Controls.Add(lblRespuesta);
            this.Controls.Add(txtRespuesta);
        }

        private void CrearCampoTexto(Control contenedor, string texto, int y, out TextBox caja, bool esPass = false)
        {
            Label lbl = new Label { Text = texto, Location = new Point(20, y + 4), AutoSize = true };
            caja = new TextBox { Location = new Point(150, y), Width = 250, UseSystemPasswordChar = esPass };
            contenedor.Controls.Add(lbl);
            contenedor.Controls.Add(caja);
        }

        private void CrearCombo(Control contenedor, string texto, int y, out ComboBox combo, string[] opciones)
        {
            Label lbl = new Label { Text = texto, Location = new Point(20, y + 4), AutoSize = true };
            combo = new ComboBox { Location = new Point(150, y), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            combo.Items.AddRange(opciones);
            combo.SelectedIndex = 0;
            contenedor.Controls.Add(lbl);
            contenedor.Controls.Add(combo);
        }

        private void btnAutenticar_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ServicioAutenticacionSoapClient cliente = new ServicioAutenticacionSoapClient();
            try
            {
                string usuarioCifrado = CifradoAes.Cifrar(txtAuthUsuario.Text.Trim());
                string passCifrada = CifradoAes.Cifrar(txtAuthPass.Text);
                string tipo = cmbAuthTipo.SelectedItem.ToString().Substring(0, 1);

                RespuestaLogin res = cliente.Autenticar(usuarioCifrado, passCifrada, tipo);
                txtRespuesta.Text = "Resultado: " + res.Resultado + "\r\nId: " + res.Id + "\r\nMensaje: " + res.Mensaje;
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexión: " + ex.Message;
            }
            finally
            {
                try { cliente.Close(); } catch { cliente.Abort(); }
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ServicioAutenticacionSoapClient cliente = new ServicioAutenticacionSoapClient();
            try
            {
                string id = txtCrearId.Text.Trim();
                string nombre = txtCrearNombre.Text.Trim();
                string ape1 = txtCrearApe1.Text.Trim();
                string ape2 = txtCrearApe2.Text.Trim();
                string correo = txtCrearCorreo.Text.Trim();
                string user = CifradoAes.Cifrar(txtCrearUsuario.Text.Trim());
                string pass = CifradoAes.Cifrar(txtCrearPass.Text);
                string estado = cmbCrearEstado.SelectedItem.ToString().Substring(0, 1);
                string tipo = cmbCrearTipo.SelectedItem.ToString().Substring(0, 1);

                Respuesta res = cliente.CrearUsuario(id, nombre, ape1, ape2, correo, user, pass, estado, tipo);
                txtRespuesta.Text = "Resultado: " + res.Resultado + "\r\nMensaje: " + res.Mensaje;
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexión: " + ex.Message;
            }
            finally
            {
                try { cliente.Close(); } catch { cliente.Abort(); }
                this.Cursor = Cursors.Default;
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ServicioAutenticacionSoapClient cliente = new ServicioAutenticacionSoapClient();
            try
            {
                string id = string.IsNullOrWhiteSpace(txtModId.Text) ? null : txtModId.Text.Trim();
                string nombre = string.IsNullOrWhiteSpace(txtModNombre.Text) ? null : txtModNombre.Text.Trim();
                string ape1 = string.IsNullOrWhiteSpace(txtModApe1.Text) ? null : txtModApe1.Text.Trim();
                string ape2 = string.IsNullOrWhiteSpace(txtModApe2.Text) ? null : txtModApe2.Text.Trim();
                string correo = string.IsNullOrWhiteSpace(txtModCorreo.Text) ? null : txtModCorreo.Text.Trim();

                string user = string.IsNullOrWhiteSpace(txtModUsuario.Text) ? null : CifradoAes.Cifrar(txtModUsuario.Text.Trim());
                string pass = string.IsNullOrEmpty(txtModPass.Text) ? null : CifradoAes.Cifrar(txtModPass.Text);

                Respuesta res = cliente.ModificarUsuario(id, nombre, ape1, ape2, correo, user, pass);
                txtRespuesta.Text = "Resultado: " + res.Resultado + "\r\nMensaje: " + res.Mensaje;
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexión: " + ex.Message;
            }
            finally
            {
                try { cliente.Close(); } catch { cliente.Abort(); }
                this.Cursor = Cursors.Default;
            }
        }

        private void btnEstado_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ServicioAutenticacionSoapClient cliente = new ServicioAutenticacionSoapClient();
            try
            {
                string id = txtEstId.Text.Trim();
                string estado = cmbEstEstado.SelectedItem.ToString().Substring(0, 1);

                Respuesta res = cliente.CambiarEstadoUsuario(id, estado);
                txtRespuesta.Text = "Resultado: " + res.Resultado + "\r\nMensaje: " + res.Mensaje;
            }
            catch (Exception ex)
            {
                txtRespuesta.Text = "Error de conexión: " + ex.Message;
            }
            finally
            {
                try { cliente.Close(); } catch { cliente.Abort(); }
                this.Cursor = Cursors.Default;
            }
        }
    }
}