using System;
using System.Globalization;
using System.ServiceModel;
using System.Web.Mvc;
using WebAdmin.Models;
using WebAdmin.WsAlmacenRef;
using WebAdmin.WsConsultasAlmacenRef;

namespace WebAdmin.Controllers
{
    // Controlador para consulta, registro y modificacion de productos (Historia ADM2)
    public class ProductosController : Controller
    {
        // GET: Productos (lista y filtra productos mediante WsConsultasAlmacen)
        [HttpGet]
        public ActionResult Index(string filtro)
        {
            ViewBag.Filtro = filtro;
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                ProductoDto[] productos = cliente.ListarProductos(filtro);
                cliente.Close();
                return View(productos ?? new ProductoDto[0]);
            }
            catch (FaultException ex)
            {
                cliente.Abort();
                // Muestra el mensaje exacto del servicio SOAP (Regla 49)
                ViewBag.ErrorServicio = ex.Message;
                return View(new ProductoDto[0]);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo conectar con el servicio de consultas de almacen.";
                return View(new ProductoDto[0]);
            }
        }

        // GET: Productos/Crear (muestra formulario vacio para producto nuevo)
        [HttpGet]
        public ActionResult Crear()
        {
            ProductoViewModel model = new ProductoViewModel
            {
                EsEdicion = false
            };
            return View("Formulario", model);
        }

        // POST: Productos/Crear (valida por campo y envia transaccion '1' a WsAlmacen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ProductoViewModel model)
        {
            model.EsEdicion = false;

            // Si hay errores de validacion no llama a ningun servicio (Regla 49)
            if (!ModelState.IsValid)
            {
                return View("Formulario", model);
            }

            decimal precioDecimal = decimal.Parse(
                model.Precio.Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture);

            ServicioAlmacenSoapClient cliente = new ServicioAlmacenSoapClient();
            try
            {
                Respuesta resp = cliente.MantenimientoProducto(
                    "1",
                    model.NoProducto.Trim(),
                    model.Nombre.Trim(),
                    precioDecimal);
                cliente.Close();

                if (resp != null && resp.Resultado)
                {
                    TempData["MensajeExito"] = resp.Mensaje;
                    return RedirectToAction("Index");
                }

                // Muestra completo el mensaje devuelto por el servicio (Regla 49)
                ViewBag.ErrorServicio = resp != null ? resp.Mensaje : "Error desconocido en el servicio de almacen.";
                return View("Formulario", model);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo conectar con el servicio de mantenimiento de almacen.";
                return View("Formulario", model);
            }
        }

        // GET: Productos/Editar/0000000101 (carga datos actuales del producto)
        [HttpGet]
        public ActionResult Editar(string id)
        {
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                ProductoDto prod = cliente.ObtenerProducto(id);
                cliente.Close();

                if (prod == null)
                {
                    TempData["MensajeExito"] = null;
                    ViewBag.ErrorServicio = "No se encontro el producto solicitado.";
                    return RedirectToAction("Index");
                }

                ProductoViewModel model = new ProductoViewModel
                {
                    NoProducto = prod.NoProducto,
                    Nombre = prod.Nombre,
                    Precio = prod.Precio,
                    EsEdicion = true
                };
                return View("Formulario", model);
            }
            catch (FaultException ex)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = ex.Message;
                return View("Formulario", new ProductoViewModel { NoProducto = id, EsEdicion = true });
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo consultar el producto en el almacen.";
                return View("Formulario", new ProductoViewModel { NoProducto = id, EsEdicion = true });
            }
        }

        // POST: Productos/Editar (valida por campo y envia transaccion '2' a WsAlmacen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ProductoViewModel model)
        {
            model.EsEdicion = true;

            // Si hay errores de validacion no llama a ningun servicio (Regla 49)
            if (!ModelState.IsValid)
            {
                return View("Formulario", model);
            }

            decimal precioDecimal = decimal.Parse(
                model.Precio.Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture);

            ServicioAlmacenSoapClient cliente = new ServicioAlmacenSoapClient();
            try
            {
                Respuesta resp = cliente.MantenimientoProducto(
                    "2",
                    model.NoProducto.Trim(),
                    model.Nombre.Trim(),
                    precioDecimal);
                cliente.Close();

                if (resp != null && resp.Resultado)
                {
                    TempData["MensajeExito"] = resp.Mensaje;
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorServicio = resp != null ? resp.Mensaje : "Error desconocido en el servicio de almacen.";
                return View("Formulario", model);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo conectar con el servicio de mantenimiento de almacen.";
                return View("Formulario", model);
            }
        }
    }
}