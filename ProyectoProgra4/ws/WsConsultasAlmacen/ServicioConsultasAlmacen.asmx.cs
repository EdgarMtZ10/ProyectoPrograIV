using System.Web.Services;

namespace WsConsultasAlmacen
{
    // Servicio SOAP de consultas de solo lectura para el modulo de almacen
    [WebService(Namespace = "http://novaonline.cr/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ServicioConsultasAlmacen : WebService
    {
        // Instancia del repositorio de acceso a SQL Server
        private readonly Repositorio repositorio = new Repositorio();

        // Devuelve el arreglo de productos filtrados
        [WebMethod]
        public ProductoDto[] ListarProductos(string filtro)
        {
            return repositorio.ListarProductos(filtro);
        }

        // Devuelve un producto puntual por numero de producto
        [WebMethod]
        public ProductoDto ObtenerProducto(string noProducto)
        {
            return repositorio.ObtenerProducto(noProducto);
        }

        // Devuelve el arreglo de proveedores filtrados
        [WebMethod]
        public ProveedorDto[] ListarProveedores(string filtro)
        {
            return repositorio.ListarProveedores(filtro);
        }

        // Devuelve un proveedor puntual por cedula juridica
        [WebMethod]
        public ProveedorDto ObtenerProveedor(string cedulaJuridica)
        {
            return repositorio.ObtenerProveedor(cedulaJuridica);
        }

        // Devuelve las ultimas 10 compras que coinciden con el filtro
        [WebMethod]
        public CompraResumenDto[] ListarUltimasCompras(string filtro)
        {
            return repositorio.ListarUltimasCompras(filtro);
        }

        // Devuelve el encabezado y detalle de una compra especifica
        [WebMethod]
        public CompraDetalleDto ObtenerCompra(string numeroIngreso)
        {
            return repositorio.ObtenerCompra(numeroIngreso);
        }
    }
}