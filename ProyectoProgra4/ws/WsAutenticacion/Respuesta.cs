
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsAutenticacion
{
    public class Respuesta
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
    }

    // Respuesta del login:
    // Devuelve Resultado, Id, Correo y Mensaje.
    public class RespuestaLogin
    {
        public bool Resultado { get; set; }
        public string Id { get; set; }

        //  correo electrónico del usuario.
        public string Correo { get; set; }

        public string Mensaje { get; set; }
    }
}
