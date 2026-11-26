
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WsAutenticacion
{
    public class Validador
    {
        // Contraseña de exactamente 14 caracteres.
        public const int LargoContrasena = 14;

        public const string EstadoActivo = "1";
        public const string EstadoInactivo = "2";

        // Letras con acentos y espacios entre palabras.
        private static readonly Regex RxNombre =
            new Regex(@"^\p{L}+( \p{L}+)*$");

        private static readonly Regex RxCorreo =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        // Validar nombre y apellidos.
        // Máximo 20 letras por campo.
        public static bool NombreValido(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
                return false;

            if (!RxNombre.IsMatch(v))
                return false;

            // Contar solamente letras, sin espacios.
            int cantidadLetras = v.Count(char.IsLetter);

            return cantidadLetras <= 20;
        }

        // Validar formato de correo electrónico.
        public static bool CorreoValido(string v)
        {
            return v != null
                && v.Length <= 100
                && RxCorreo.IsMatch(v);
        }

        // Validar identificación de Costa Rica.
        // Física: 9 dígitos.
        // Jurídica: 10 dígitos.
        public static bool IdentificacionValida(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
                return false;

            return Regex.IsMatch(v, @"^([0-9]{9}|[0-9]{10})$");
        }

        // Validar nombre de usuario.
        public static bool UsuarioValido(string v)
        {
            return !string.IsNullOrWhiteSpace(v)
                && v.Length <= 50
                && v.IndexOf(' ') < 0;
        }

        // Contraseña de 14 caracteres:
        // 1 mayúscula, 1 minúscula, 1 número y 1 especial.
        public static bool ContrasenaValida(string v)
        {
            if (v == null || v.Length != LargoContrasena)
                return false;

            return v.Any(char.IsUpper)
                && v.Any(char.IsLower)
                && v.Any(char.IsDigit)
                && v.Any(c =>
                    !char.IsLetterOrDigit(c)
                    && !char.IsWhiteSpace(c));
        }

        // Estado: 1 activo, 2 inactivo.
        public static bool EstadoValido(string v)
        {
            return v == EstadoActivo
                || v == EstadoInactivo;
        }

        // Tipo: 1 empleado, 2 cliente.
        public static bool TipoValido(string v)
        {
            return v == "1" || v == "2";
        }
    }
}
