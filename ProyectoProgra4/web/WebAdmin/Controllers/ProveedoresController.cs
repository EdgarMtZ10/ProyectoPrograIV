using System;
using System.ServiceModel;
using System.Web.Mvc;
using WebAdmin.Models;
using WebAdmin.WsAlmacenRef;
using WebAdmin.WsConsultasAlmacenRef;

namespace WebAdmin.Controllers
{
    // Controlador para consulta, registro y modificacion de proveedores (Historia ADM3)
    public class ProveedoresController : Controller
    {
        // GET: Proveedores (lista y filtra proveedores mediante WsConsultasAlmacen)
        [HttpGet]
        public ActionResult Index(string filtro)
        {
            ViewBag.Filtro = filtro;
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                ProveedorDto[] proveedores = cliente.ListarProveedores(filtro);
                cliente.Close();
                return View(proveedores ?? new ProveedorDto[0]);
            }
            catch (FaultException ex)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = ex.Message;
                return View(new ProveedorDto[0]);
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo conectar con el servicio de consultas de almacen.";
                return View(new ProveedorDto[0]);
            }
        }

        // GET: Proveedores/Crear (muestra formulario de ingreso con estado Activo por defecto)
        [HttpGet]
        public ActionResult Crear()
        {
            ProveedorViewModel model = new ProveedorViewModel
            {
                EsEdicion = false,
                Estado = "1"
            };
            return View("Formulario", model);
        }

        // POST: Proveedores/Crear (valida por campo y envia transaccion '3' a WsAlmacen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ProveedorViewModel model)
        {
            model.EsEdicion = false;
            // Al ingresar un proveedor nuevo siempre queda activo ('1')
            model.Estado = "1";

            // Si hay errores de validacion no llama a ningun servicio (Regla 49)
            if (!ModelState.IsValid)
            {
                return View("Formulario", model);
            }

            ServicioAlmacenSoapClient cliente = new ServicioAlmacenSoapClient();
            try
            {
                Respuesta resp = cliente.MantenimientoProveedor(
                    "3",
                    model.CedulaJuridica.Trim(),
                    model.NombreEmpresa.Trim(),
                    model.NombreContacto.Trim(),
                    model.Telefono.Trim(),
                    model.Correo.Trim(),
                    "1");
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

        // GET: Proveedores/Editar/3102777777 (carga datos actuales del proveedor)
        [HttpGet]
        public ActionResult Editar(string id)
        {
            ServicioConsultasAlmacenSoapClient cliente = new ServicioConsultasAlmacenSoapClient();
            try
            {
                ProveedorDto prov = cliente.ObtenerProveedor(id);
                cliente.Close();

                if (prov == null)
                {
                    return RedirectToAction("Index");
                }

                ProveedorViewModel model = new ProveedorViewModel
                {
                    CedulaJuridica = prov.CedulaJuridica,
                    NombreEmpresa = prov.Nombre,
                    NombreContacto = prov.NombreContacto,
                    Telefono = prov.Telefono,
                    Correo = prov.Correo,
                    Estado = prov.Estado,
                    EsEdicion = true
                };
                return View("Formulario", model);
            }
            catch (FaultException ex)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = ex.Message;
                return View("Formulario", new ProveedorViewModel { CedulaJuridica = id, EsEdicion = true });
            }
            catch (Exception)
            {
                cliente.Abort();
                ViewBag.ErrorServicio = "No se pudo consultar el proveedor en el almacen.";
                return View("Formulario", new ProveedorViewModel { CedulaJuridica = id, EsEdicion = true });
            }
        }

        // POST: Proveedores/Editar (valida por campo y envia transaccion '4' a WsAlmacen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ProveedorViewModel model)
        {
            model.EsEdicion = true;

            // Si hay errores de validacion no llama a ningun servicio (Regla 49)
            if (!ModelState.IsValid)
            {
                return View("Formulario", model);
            }

            ServicioAlmacenSoapClient cliente = new ServicioAlmacenSoapClient();
            try
            {
                Respuesta resp = cliente.MantenimientoProveedor(
                    "4",
                    model.CedulaJuridica.Trim(),
                    model.NombreEmpresa.Trim(),
                    model.NombreContacto.Trim(),
                    model.Telefono.Trim(),
                    model.Correo.Trim(),
                    model.Estado.Trim());
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