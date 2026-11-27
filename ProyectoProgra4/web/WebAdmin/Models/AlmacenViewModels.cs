using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebAdmin.Models
{
    // ViewModel para crear y editar productos (Historia ADM2)
    public class ProductoViewModel : IValidatableObject
    {
        [Display(Name = "Numero de producto")]
        public string NoProducto { get; set; }

        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; }

        [Display(Name = "Precio")]
        public string Precio { get; set; }

        // Indica si el formulario corresponde a edicion (true) o ingreso nuevo (false)
        public bool EsEdicion { get; set; }

        // Ejecuta validaciones por campo en el servidor antes de llamar a WsAlmacen
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string codigo = (NoProducto ?? string.Empty).Trim();
            if (!Regex.IsMatch(codigo, @"^\d{10}$"))
            {
                yield return new ValidationResult(
                    "numero de producto: debe tener exactamente 10 digitos numericos.",
                    new[] { nameof(NoProducto) });
            }

            string nombreLimpio = (Nombre ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(nombreLimpio))
            {
                yield return new ValidationResult(
                    "nombre del producto: es obligatorio.",
                    new[] { nameof(Nombre) });
            }
            else if (nombreLimpio.Length > 90)
            {
                yield return new ValidationResult(
                    "nombre del producto: maximo 90 caracteres.",
                    new[] { nameof(Nombre) });
            }
            else if (Regex.IsMatch(nombreLimpio, @"^\d+$"))
            {
                yield return new ValidationResult(
                    "nombre del producto: no puede ser solo numeros.",
                    new[] { nameof(Nombre) });
            }

            string precioLimpio = (Precio ?? string.Empty).Trim().Replace(',', '.');
            decimal precioValor;
            if (string.IsNullOrEmpty(precioLimpio))
            {
                yield return new ValidationResult(
                    "precio: es obligatorio.",
                    new[] { nameof(Precio) });
            }
            else if (!Regex.IsMatch(precioLimpio, @"^\d{1,6}(\.\d{1,2})?$")
                     || !decimal.TryParse(precioLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out precioValor)
                     || precioValor <= 0m
                     || precioValor > 999999.99m)
            {
                yield return new ValidationResult(
                    "precio: debe ser positivo, con maximo 2 decimales y hasta 999999.99.",
                    new[] { nameof(Precio) });
            }
        }
    }

    // ViewModel para crear y editar proveedores (Historia ADM3)
    public class ProveedorViewModel : IValidatableObject
    {
        [Display(Name = "Cedula juridica")]
        public string CedulaJuridica { get; set; }

        [Display(Name = "Nombre de la empresa")]
        public string NombreEmpresa { get; set; }

        [Display(Name = "Nombre del contacto")]
        public string NombreContacto { get; set; }

        [Display(Name = "Telefono")]
        public string Telefono { get; set; }

        [Display(Name = "Correo electronico")]
        public string Correo { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        // Indica si el formulario corresponde a modificacion (true) o registro nuevo (false)
        public bool EsEdicion { get; set; }

        // Ejecuta validaciones por campo en el servidor antes de llamar a WsAlmacen
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string cedula = (CedulaJuridica ?? string.Empty).Trim();
            if (!Regex.IsMatch(cedula, @"^\d{10}$"))
            {
                yield return new ValidationResult(
                    "cedula juridica: debe tener exactamente 10 digitos numericos.",
                    new[] { nameof(CedulaJuridica) });
            }

            string empresa = (NombreEmpresa ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(empresa))
            {
                yield return new ValidationResult(
                    "nombre de la empresa: es obligatorio.",
                    new[] { nameof(NombreEmpresa) });
            }
            else if (empresa.Length > 100)
            {
                yield return new ValidationResult(
                    "nombre de la empresa: maximo 100 caracteres.",
                    new[] { nameof(NombreEmpresa) });
            }
            else if (Regex.IsMatch(empresa, @"^\d+$"))
            {
                yield return new ValidationResult(
                    "nombre de la empresa: no puede ser solo numeros.",
                    new[] { nameof(NombreEmpresa) });
            }

            string contacto = (NombreContacto ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(contacto))
            {
                yield return new ValidationResult(
                    "nombre del contacto: es obligatorio.",
                    new[] { nameof(NombreContacto) });
            }
            else if (contacto.Length > 75)
            {
                yield return new ValidationResult(
                    "nombre del contacto: maximo 75 caracteres.",
                    new[] { nameof(NombreContacto) });
            }
            else if (Regex.IsMatch(contacto, @"^\d+$"))
            {
                yield return new ValidationResult(
                    "nombre del contacto: no puede ser solo numeros.",
                    new[] { nameof(NombreContacto) });
            }

            string tel = (Telefono ?? string.Empty).Trim();
            if (!Regex.IsMatch(tel, @"^\d{8}$"))
            {
                yield return new ValidationResult(
                    "telefono: debe tener exactamente 8 digitos numericos.",
                    new[] { nameof(Telefono) });
            }

            string correoLimpio = (Correo ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(correoLimpio))
            {
                yield return new ValidationResult(
                    "correo: es obligatorio.",
                    new[] { nameof(Correo) });
            }
            else if (correoLimpio.Length > 75)
            {
                yield return new ValidationResult(
                    "correo: maximo 75 caracteres.",
                    new[] { nameof(Correo) });
            }
            else if (!Regex.IsMatch(correoLimpio, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                yield return new ValidationResult(
                    "correo: formato de correo electronico invalido.",
                    new[] { nameof(Correo) });
            }

            string estadoLimpio = (Estado ?? string.Empty).Trim();
            if (estadoLimpio != "1" && estadoLimpio != "2")
            {
                yield return new ValidationResult(
                    "estado: debe ser Activo (1) o Inactivo (2).",
                    new[] { nameof(Estado) });
            }
        }
    }

    // Representa cada linea de producto dentro de una compra a proveedor (Historia ADM4)
    public class CompraLineaViewModel
    {
        public string NoProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Cantidad { get; set; }
    }

    // ViewModel para registrar una compra a proveedor con 1 o mas productos (Historia ADM4)
    public class CompraRegistroViewModel : IValidatableObject
    {
        [Display(Name = "Numero de ingreso")]
        public string NumeroIngreso { get; set; }

        [Display(Name = "Fecha de compra")]
        public string FechaCompra { get; set; }

        [Display(Name = "Cedula juridica")]
        public string CedulaJuridica { get; set; }

        [Display(Name = "Nombre de la empresa")]
        public string NombreEmpresa { get; set; }

        // Lista de productos agregados a la compra
        public List<CompraLineaViewModel> Lineas { get; set; } = new List<CompraLineaViewModel>();

        // Valida encabezado y cada linea en el servidor
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string ingreso = (NumeroIngreso ?? string.Empty).Trim();
            long ingresoNum;
            if (string.IsNullOrEmpty(ingreso) || ingreso.Length > 10 || !Regex.IsMatch(ingreso, @"^\d+$")
                || !long.TryParse(ingreso, NumberStyles.None, CultureInfo.InvariantCulture, out ingresoNum)
                || ingresoNum <= 0)
            {
                yield return new ValidationResult(
                    "numero de ingreso: es obligatorio, solo digitos, mayor a cero y maximo 10 caracteres.",
                    new[] { nameof(NumeroIngreso) });
            }

            string fechaTexto = (FechaCompra ?? string.Empty).Trim();
            DateTime fechaParseada;
            if (string.IsNullOrEmpty(fechaTexto))
            {
                yield return new ValidationResult(
                    "fecha de compra: es obligatoria.",
                    new[] { nameof(FechaCompra) });
            }
            else if (!DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParseada))
            {
                yield return new ValidationResult(
                    "fecha de compra: debe tener formato dd/MM/yyyy.",
                    new[] { nameof(FechaCompra) });
            }
            else if (fechaParseada.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "fecha de compra: no puede ser mayor a la fecha actual.",
                    new[] { nameof(FechaCompra) });
            }

            string cedula = (CedulaJuridica ?? string.Empty).Trim();
            if (!Regex.IsMatch(cedula, @"^\d{10}$"))
            {
                yield return new ValidationResult(
                    "cedula juridica: debe tener exactamente 10 digitos numericos.",
                    new[] { nameof(CedulaJuridica) });
            }

            if (Lineas == null || Lineas.Count == 0)
            {
                yield return new ValidationResult(
                    "Debe agregar al menos un producto a la compra.",
                    new[] { nameof(Lineas) });
                yield break;
            }

            for (int i = 0; i < Lineas.Count; i++)
            {
                CompraLineaViewModel linea = Lineas[i];
                string cod = (linea != null ? linea.NoProducto : string.Empty ?? string.Empty).Trim();
                string cantStr = (linea != null ? linea.Cantidad : string.Empty ?? string.Empty).Trim();

                if (!Regex.IsMatch(cod, @"^\d{10}$"))
                {
                    yield return new ValidationResult(
                        "numero de producto en linea " + (i + 1) + ": debe tener exactamente 10 digitos numericos.",
                        new[] { "Lineas[" + i + "].NoProducto" });
                }

                int cantVal;
                if (string.IsNullOrEmpty(cantStr) || cantStr.Length > 7 || !Regex.IsMatch(cantStr, @"^\d+$")
                    || !int.TryParse(cantStr, NumberStyles.None, CultureInfo.InvariantCulture, out cantVal)
                    || cantVal <= 0)
                {
                    yield return new ValidationResult(
                        "cantidad en linea " + (i + 1) + ": es obligatoria, solo digitos, mayor a cero y maximo 7 digitos.",
                        new[] { "Lineas[" + i + "].Cantidad" });
                }
            }
        }
    }
}