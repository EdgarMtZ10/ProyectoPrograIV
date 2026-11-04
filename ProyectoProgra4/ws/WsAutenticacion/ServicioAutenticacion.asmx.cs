using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WsAutenticacion
{
    [WebService(Namespace = "http://novaonline.cr/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class ServicioAutenticacion : System.Web.Services.WebService
    {

        private const string MsgExitoso = "Exitoso";
        private const string MsgLoginFallo = "Usuario y/o contraseña incorrectos.";
        private const string MsgCrearFallo = "Usuario ya existe o datos incorrectos o incompletos.";
        private const string MsgModificarFallo = "Usuario no existe o datos incorrectos o incompletos.";
        private const string MsgEstadoFallo = "Usuario no existe o datos incorrectos.";

        [WebMethod]
        public string Ping()
        {
            return "WsAutenticacion activo";
        }

        //  WS_AUTENTICACION1
        [WebMethod]
        public RespuestaLogin Autenticar(string usuario, string contrasena, string tipoUsuario)
        {
            var resp = new RespuestaLogin { Resultado = false, Id = "", Mensaje = MsgLoginFallo };
            try
            {
                string u = Descifrar(usuario);
                string p = Descifrar(contrasena);

                if (!string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(p) && Validador.TipoValido(tipoUsuario))
                {
                    var repo = new UsuarioRepositorio();
                    BsonDocument doc = repo.BuscarCredenciales(
                        CifradoAes.Cifrar(u), CifradoAes.Cifrar(p), int.Parse(tipoUsuario));

                    if (doc != null)
                    {
                        resp.Resultado = true;
                        resp.Id = doc["identificacion"].ToString();
                        resp.Mensaje = MsgExitoso;
                    }
                }
            }
            catch
            {
                resp.Resultado = false;
                resp.Id = "";
                resp.Mensaje = MsgLoginFallo;
            }

            var solicitud = new Dictionary<string, object>
            {
                { "usuario", ParaBitacora(usuario) },
                { "contrasena", ParaBitacora(contrasena) },
                { "tipoUsuario", tipoUsuario }
            };
            var respuesta = new Dictionary<string, object>
            {
                { "Resultado", resp.Resultado },
                { "Id", resp.Id },
                { "Mensaje", resp.Mensaje }
            };
            Bitacora.Registrar("Autenticar", solicitud, respuesta);
            return resp;
        }

        // WS_AUTENTICACION2 
        [WebMethod]
        public Respuesta CrearUsuario(string identificacion, string nombre, string primerApellido,
            string segundoApellido, string correo, string usuario, string contrasena, string estado, string tipo)
        {
            var resp = new Respuesta { Resultado = false, Mensaje = MsgCrearFallo };
            try
            {
                string u = Descifrar(usuario);
                string p = Descifrar(contrasena);

                bool datosValidos =
                    Validador.IdentificacionValida(identificacion)
                    && Validador.NombreValido(nombre)
                    && Validador.NombreValido(primerApellido)
                    && Validador.NombreValido(segundoApellido)
                    && Validador.CorreoValido(correo)
                    && Validador.UsuarioValido(u)
                    && Validador.ContrasenaValida(p)
                    && estado == Validador.EstadoActivo   // usuarios nuevos: solo activo
                    && Validador.TipoValido(tipo);

                if (datosValidos)
                {
                    string uCif = CifradoAes.Cifrar(u);
                    string pCif = CifradoAes.Cifrar(p);
                    var repo = new UsuarioRepositorio();

                    if (!repo.ExisteIdentificacionOUsuario(identificacion, uCif))
                    {
                        repo.Insertar(new BsonDocument
                        {
                            { "identificacion", identificacion },
                            { "nombre", nombre },
                            { "primerApellido", primerApellido },
                            { "segundoApellido", segundoApellido },
                            { "correo", correo },
                            { "usuario", uCif },
                            { "contrasena", pCif },
                            { "estado", 1 },
                            { "tipo", int.Parse(tipo) }
                        });
                        resp.Resultado = true;
                        resp.Mensaje = MsgExitoso;
                    }
                }
            }
            catch
            {
                resp.Resultado = false;
                resp.Mensaje = MsgCrearFallo;
            }

            var solicitud = new Dictionary<string, object>
            {
                { "identificacion", identificacion },
                { "nombre", nombre },
                { "primerApellido", primerApellido },
                { "segundoApellido", segundoApellido },
                { "correo", correo },
                { "usuario", ParaBitacora(usuario) },
                { "contrasena", ParaBitacora(contrasena) },
                { "estado", estado },
                { "tipo", tipo }
            };
            Bitacora.Registrar("CrearUsuario", solicitud, RespuestaADiccionario(resp));
            return resp;
        }

        // Campo null = ausente (no se modifica). Campo presente pero en blanco o inválido = error.
        [WebMethod]
        public Respuesta ModificarUsuario(string identificacion, string nombre, string primerApellido,
            string segundoApellido, string correo, string usuario, string contrasena)
        {
            var resp = new Respuesta { Resultado = false, Mensaje = MsgModificarFallo };
            try
            {
                if (Validador.IdentificacionValida(identificacion))
                {
                    var repo = new UsuarioRepositorio();
                    if (repo.Obtener(identificacion) != null)
                    {
                        var ub = Builders<BsonDocument>.Update;
                        var cambios = new List<UpdateDefinition<BsonDocument>>();
                        bool valido = true;

                        if (nombre != null)
                        {
                            if (Validador.NombreValido(nombre)) cambios.Add(ub.Set("nombre", nombre));
                            else valido = false;
                        }
                        if (primerApellido != null)
                        {
                            if (Validador.NombreValido(primerApellido)) cambios.Add(ub.Set("primerApellido", primerApellido));
                            else valido = false;
                        }
                        if (segundoApellido != null)
                        {
                            if (Validador.NombreValido(segundoApellido)) cambios.Add(ub.Set("segundoApellido", segundoApellido));
                            else valido = false;
                        }
                        if (correo != null)
                        {
                            if (Validador.CorreoValido(correo)) cambios.Add(ub.Set("correo", correo));
                            else valido = false;
                        }
                        if (usuario != null)
                        {
                            string u = Descifrar(usuario);
                            if (Validador.UsuarioValido(u))
                            {
                                string uCif = CifradoAes.Cifrar(u);
                                if (repo.UsuarioEnUsoPorOtro(uCif, identificacion)) valido = false;
                                else cambios.Add(ub.Set("usuario", uCif));
                            }
                            else valido = false;
                        }
                        if (contrasena != null)
                        {
                            string p = Descifrar(contrasena);
                            if (Validador.ContrasenaValida(p)) cambios.Add(ub.Set("contrasena", CifradoAes.Cifrar(p)));
                            else valido = false;
                        }

                        if (valido && cambios.Count > 0)
                        {
                            repo.Actualizar(identificacion, ub.Combine(cambios));
                            resp.Resultado = true;
                            resp.Mensaje = MsgExitoso;
                        }
                    }
                }
            }
            catch
            {
                resp.Resultado = false;
                resp.Mensaje = MsgModificarFallo;
            }

            var solicitud = new Dictionary<string, object>
            {
                { "identificacion", identificacion },
                { "nombre", nombre },
                { "primerApellido", primerApellido },
                { "segundoApellido", segundoApellido },
                { "correo", correo },
                { "usuario", ParaBitacora(usuario) },
                { "contrasena", ParaBitacora(contrasena) }
            };
            Bitacora.Registrar("ModificarUsuario", solicitud, RespuestaADiccionario(resp));
            return resp;
        }

        [WebMethod]
        public Respuesta CambiarEstadoUsuario(string identificacion, string estado)
        {
            var resp = new Respuesta { Resultado = false, Mensaje = MsgEstadoFallo };
            try
            {
                if (Validador.IdentificacionValida(identificacion) && Validador.EstadoValido(estado))
                {
                    var repo = new UsuarioRepositorio();
                    if (repo.Obtener(identificacion) != null)
                    {
                        repo.Actualizar(identificacion, Builders<BsonDocument>.Update.Set("estado", int.Parse(estado)));
                        resp.Resultado = true;
                        resp.Mensaje = MsgExitoso;
                    }
                }
            }
            catch
            {
                resp.Resultado = false;
                resp.Mensaje = MsgEstadoFallo;
            }

            var solicitud = new Dictionary<string, object>
            {
                { "identificacion", identificacion },
                { "estado", estado }
            };
            Bitacora.Registrar("CambiarEstadoUsuario", solicitud, RespuestaADiccionario(resp));
            return resp;
        }

        // Auxiliares 

        // Devuelve null si el valor no viene o no es un texto cifrado válido.
        private static string Descifrar(string cifrado)
        {
            try
            {
                if (string.IsNullOrEmpty(cifrado)) return null;
                return CifradoAes.Descifrar(cifrado);
            }
            catch
            {
                return null;
            }
        }

        // En la bitácora usuario y contraseña siempre van cifrados: si lo recibido
        // no es un cifrado válido (llegó en claro), se cifra antes de escribirlo.
        private static string ParaBitacora(string recibido)
        {
            if (string.IsNullOrEmpty(recibido)) return recibido;
            if (Descifrar(recibido) != null) return recibido;
            try { return CifradoAes.Cifrar(recibido); }
            catch { return "[no registrable]"; }
        }

        private static Dictionary<string, object> RespuestaADiccionario(Respuesta r)
        {
            return new Dictionary<string, object>
            {
                { "Resultado", r.Resultado },
                { "Mensaje", r.Mensaje }
            };
        }
    }
}
