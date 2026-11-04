using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WsAutenticacion
{
    public class Validador
    {
        // El PDF dice "14 dígitos"; se interpreta como 14 caracteres.
        public const int LargoContrasena = 14;

        public const string EstadoActivo = "1";
        public const string EstadoInactivo = "2";

        private static readonly Regex RxNombre = new Regex(@"^\p{L}+( \p{L}+)*\z");
        private static readonly Regex RxCorreo = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+\z");

        // Letras (con acentos) y un solo espacio entre palabras. Sin vacíos, sin números.
        public static bool NombreValido(string v)
        {
            return v != null && RxNombre.IsMatch(v);
        }

        public static bool CorreoValido(string v)
        {
            return v != null && v.Length <= 100 && RxCorreo.IsMatch(v);
        }

        public static bool IdentificacionValida(string v)
        {
            return !string.IsNullOrWhiteSpace(v) && v.Length <= 30 && v.All(c => char.IsLetterOrDigit(c) || c == '-');
        }

        public static bool UsuarioValido(string v)
        {
            return !string.IsNullOrWhiteSpace(v) && v.Length <= 50 && v.IndexOf(' ') < 0;
        }

        // 14 caracteres con al menos: 1 mayúscula, 1 minúscula, 1 número, 1 especial
        public static bool ContrasenaValida(string v)
        {
            if (v == null || v.Length != LargoContrasena) return false;
            return v.Any(char.IsUpper)
                && v.Any(char.IsLower)
                && v.Any(char.IsDigit)
                && v.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }

        // Estado: "1" activo, "2" inactivo
        public static bool EstadoValido(string v)
        {
            return v == EstadoActivo || v == EstadoInactivo;
        }

        // Tipo: "1" empleado, "2" cliente
        public static bool TipoValido(string v)
        {
            return v == "1" || v == "2";
        }
    }
}