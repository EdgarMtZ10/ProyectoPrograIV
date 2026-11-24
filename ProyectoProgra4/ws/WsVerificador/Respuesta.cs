using System;

namespace WsVerificador
{
    // Estructura estándar de respuesta del servicio igual a WsAlmacen y WsAutenticacion
    public class Respuesta
    {
        public bool Resultado { get; set; }
        public string Mensaje { get; set; }
    }
}