using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace simulador2
{
    public static class CifradoAes
    {
        // Llave de 32 caracteres
        private static readonly string Clave = "12345678901234567890123456789012";

        // IV de 16 caracteres
        private static readonly string IV = "1234567890123456";

        // Método para cifrar textos
        public static string Cifrar(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return textoPlano;

            byte[] arregloBytes = Encoding.UTF8.GetBytes(textoPlano);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Clave);
                aes.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform encriptador = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encriptador, CryptoStreamMode.Write))
                    {
                        cs.Write(arregloBytes, 0, arregloBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Método para descifrar los textos
        public static string Descifrar(string textoCifrado)
        {
            if (string.IsNullOrEmpty(textoCifrado)) return textoCifrado;

            byte[] arregloBytes = Convert.FromBase64String(textoCifrado);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Clave);
                aes.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform desencriptador = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(arregloBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, desencriptador, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}