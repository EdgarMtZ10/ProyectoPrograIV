using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using WebAdmin.Models;
using WebAdmin.WsAlmacenRef;
using WebAdmin.WsConsultasAlmacenRef;

namespace WebAdmin.Controllers
{
    // Controlador para consulta y registro de compras a proveedores (Historia ADM4)
    public class ComprasController : Controller
    {
        // GET: Compras (lista las ultimas 10 compras filtradas)
        [HttpGet]
        public ActionResult Index(string filtro)
        {
            ViewBag.Filtro = filtro;
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                CompraResumenDto[] compras = cliente.ListarUltimasCompras(filtro);
                cliente.Close();
                return View(compras ?? new CompraResumenDto[0]);
            }
            catch (FaultException ex)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = ex.Message;
                return View(new CompraResumenDto[0]);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo conectar con el servicio de consultas de almacen.";
                return View(new CompraResumenDto[0]);
            }
        }

        // GET: Compras/Detalle/1001 (muestra encabezado y lineas con nombre de producto)
        [HttpGet]
        public ActionResult Detalle(string id)
        {
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                CompraDetalleDto compra = cliente.ObtenerCompra(id);
                cliente.Close();

                if (compra == null)
                {
                    ViewBag.ErrorServicio = "No se encontro la compra solicitada.";
                }
                return View(compra);
            }
            catch (FaultException ex)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = ex.Message;
                return View((CompraDetalleDto)null);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo consultar el detalle de la compra.";
                return View((CompraDetalleDto)null);
            }
        }

        // GET: Compras/Registrar (carga proveedores activos, catalogo de productos y primera fila)
        [HttpGet]
        public ActionResult Registrar()
        {
            CargarListasDesplegables();
            CompraRegistroViewModel model = new CompraRegistroViewModel
            {
                FechaCompra = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Lineas = new List<CompraLineaViewModel>
                {
                    new CompraLineaViewModel()
                }
            };
            return View(model);
        }

        // POST: Compras/Registrar (valida encabezado y todas las lineas antes de llamar a WsAlmacen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(CompraRegistroViewModel model)
        {
            if (model.Lineas == null)
            {
                model.Lineas = new List<CompraLineaViewModel>();
            }

            // Si hay errores de validacion no llama a ningun servicio (Regla 49)
            if (!ModelState.IsValid)
            {
                CargarListasDesplegables();
                return View(model);
            }

            // Convierte fecha dd/MM/yyyy a yyyyMMdd esperado por CompraParser en Java
            DateTime fechaValida = DateTime.ParseExact(
                model.FechaCompra.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture);
            string fechaTrama = fechaValida.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            // Mapea cada linea del formulario al arreglo ProductoCompra[] del proxy SOAP
            ProductoCompra[] detalles = model.Lineas
                .Select(l => new ProductoCompra
                {
                    NoProducto = l.NoProducto.Trim(),
                    Cantidad = l.Cantidad.Trim()
                })
                .ToArray();

            ServicioAlmacenSoapClient cliente = new ServicioAlmacenSoapClient();
            try
            {
                Respuesta resp = cliente.RegistrarCompra(
                    model.NumeroIngreso.Trim(),
                    fechaTrama,
                    model.CedulaJuridica.Trim(),
                    detalles);
                cliente.Close();

                if (resp != null && resp.Resultado)
                {
                    TempData["MensajeExito"] = resp.Mensaje;
                    return RedirectToAction("Index");
                }

                // Muestra completo el error devuelto por el servicio (Regla 49)
                ViewBag.ErrorServicio = resp != null ? resp.Mensaje : "Error desconocido al registrar la compra.";
                CargarListasDesplegables();
                return View(model);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo conectar con el servicio de mantenimiento de almacen.";
                CargarListasDesplegables();
                return View(model);
            }
        }

        // Consulta proveedores activos y productos disponibles para llenar los selectores
        private void CargarListasDesplegables()
        {
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                ProveedorDto[] proveedores = cliente.ListarProveedores(string.Empty) ?? new ProveedorDto[0];
                ProductoDto[] productos = cliente.ListarProductos(string.Empty) ?? new ProductoDto[0];
                cliente.Close();

                // Solo ofrece proveedores activos (estado == "1") para registrar compras
                ViewBag.Proveedores = proveedores
                    .Where(p => p.Estado == "1")
                    .Select(p => new SelectListItem
                    {
                        Value = p.CedulaJuridica,
                        Text = p.CedulaJuridica + " - " + p.Nombre
                    })
                    .ToList();

                ViewBag.Productos = productos
                    .Select(p => new SelectListItem
                    {
                        Value = p.NoProducto,
                        Text = p.NoProducto + " - " + p.Nombre
                    })
                    .ToList();
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.Proveedores = new List<SelectListItem>();
                ViewBag.Productos = new List<SelectListItem>();
                if (ViewBag.ErrorServicio == null)
                {
                    ViewBag.ErrorServicio = "No se pudieron cargar las listas de proveedores y productos.";
                }
            }
        }
    }
}