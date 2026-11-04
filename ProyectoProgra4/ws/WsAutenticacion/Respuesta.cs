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

    // Respuesta del login: agrega Id (identificación del cliente o empleado)
    public class RespuestaLogin
    {
        public bool Resultado { get; set; }
        public string Id { get; set; }
        public string Mensaje { get; set; }
    }
}