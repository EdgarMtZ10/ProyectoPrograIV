package almacen.almacen4.validaciones;

import almacen.almacen4.datos.Salida;
import almacen.almacen4.datos.SalidaDetalle;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

/**
 * Valida las reglas de negocio de una salida antes de tocar la base de
 * datos.
 * aqui no se valida la existencia de producto ni disponibilidad de inventario, eso lo hace el procedimiento almacenado.
 */
public class SalidaValidador {

    // El numero de producto debe tener exactamente 10 digitos numericos
    private static final Pattern PATRON_PRODUCTO = Pattern.compile("^\\d{10}$");

    public List<String> validar(Salida salida) {
        List<String> errores = new ArrayList<>();

        validarNumeroFactura(salida.getNumeroFactura(), errores);
        validarFechaVenta(salida.getFechaVenta(), errores);
        validarListaProductos(salida.getDetalles(), errores);

        return errores;
    }

    public boolean esValida(Salida salida) {
        return validar(salida).isEmpty();
    }

    // Valida que el numero de factura sea positivo y mayor a cero
    private void validarNumeroFactura(long numeroFactura, List<String> errores) {
        if (numeroFactura <= 0) {
            errores.add("El numero de factura debe ser un valor positivo y mayor a cero.");
        }
    }

    // Valida que la fecha de venta no sea nula y no sea a futuro con respecto a la fecha actual
    private void validarFechaVenta(LocalDate fechaVenta, List<String> errores) {
        if (fechaVenta == null) {
            errores.add("La fecha de venta no puede ser nula.");
            return;
        }
        if (fechaVenta.isAfter(LocalDate.now())) {
            errores.add("La fecha de venta no puede ser a futuro.");
        }
    }

    // Valida que la lista de productos no sea nula ni vacía, y que cada producto tenga un numero de producto valido y una cantidad mayor a cero
    private void validarListaProductos(List<SalidaDetalle> detalles, List<String> errores) {
        if (detalles == null || detalles.isEmpty()) {
            errores.add("La salida debe incluir al menos un producto.");
            return;
        }
        
        for (int i = 0; i < detalles.size(); i++) {
            SalidaDetalle detalle = detalles.get(i);

            if (detalle.getNoProducto() == null || !PATRON_PRODUCTO.matcher(detalle.getNoProducto()).matches()) {
                errores.add("El numero de producto en la posicion " + (i + 1) + " es invalido. Debe ser de 10 digitos numericos.");
            }

            if (detalle.getCantidad() <= 0) {
                errores.add("La cantidad del producto '" + detalle.getNoProducto() + "' debe ser mayor a cero.");
            }
        }
    }
}