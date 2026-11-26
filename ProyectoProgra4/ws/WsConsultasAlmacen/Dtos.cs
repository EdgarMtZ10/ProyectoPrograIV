namespace WsConsultasAlmacen
{
    // DTO con los datos de producto e inventario actual
    public class ProductoDto
    {
        public string NoProducto { get; set; }
        public string Nombre { get; set; }
        public string Precio { get; set; }
        public string Cantidad { get; set; }
    }

    // DTO con los datos generales del proveedor
    public class ProveedorDto
    {
        public string CedulaJuridica { get; set; }
        public string Nombre { get; set; }
        public string NombreContacto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Estado { get; set; }
    }

    // DTO resumen para el listado de compras a proveedores
    public class CompraResumenDto
    {
        public string NumeroIngreso { get; set; }
        public string FechaCompra { get; set; }
        public string CedulaJuridica { get; set; }
        public string NombreEmpresa { get; set; }
    }

    // DTO para cada linea de detalle de una compra
    public class LineaDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Cantidad { get; set; }
    }

    // DTO completo con encabezado y lineas de una compra
    public class CompraDetalleDto
    {
        public string NumeroIngreso { get; set; }
        public string FechaCompra { get; set; }
        public string CedulaJuridica { get; set; }
        public string NombreEmpresa { get; set; }
        public LineaDto[] Lineas { get; set; }
    }
}