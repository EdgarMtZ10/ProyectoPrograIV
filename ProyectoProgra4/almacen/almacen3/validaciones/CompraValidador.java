package almacen.almacen3.validaciones;

import almacen.almacen3.datos.Compra;
import almacen.almacen3.datos.CompraDetalle;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

/**
 * Valida las reglas de negocio de una compra antes de enviarla a la base de datos.
 */
public class CompraValidador {

    // Patron para exigir exactamente 10 digitos numericos en la cedula juridica
    private static final Pattern PATRON_CEDULA = Pattern.compile("^\\d{10}$");

    // Patron para exigir exactamente 10 digitos numericos en el numero de producto
    private static final Pattern PATRON_PRODUCTO = Pattern.compile("^\\d{10}$");

    // Revisa todas las reglas de formato de la compra y devuelve la lista de errores
    public List<String> validar(Compra compra) {
        List<String> errores = new ArrayList<>();

        validarNumeroIngreso(compra.getNumeroIngreso(), errores);
        validarFechaCompra(compra.getFechaCompra(), errores);
        validarCedulaJuridica(compra.getCedulaJuridica(), errores);
        validarListaProductos(compra.getDetalles(), errores);

        return errores;
    }

    // Indica si la compra cumple todas las validaciones
    public boolean esValida(Compra compra) {
        return validar(compra).isEmpty();
    }

    // Valida que el numero de ingreso sea positivo
    private void validarNumeroIngreso(long numeroIngreso, List<String> errores) {
        if (numeroIngreso <= 0) {
            errores.add("numero de ingreso: debe ser mayor a cero.");
        }
    }

    // Valida que la fecha de compra no sea nula ni posterior a hoy
    private void validarFechaCompra(LocalDate fechaCompra, List<String> errores) {
        if (fechaCompra == null) {
            errores.add("fecha de compra: es obligatoria.");
            return;
        }

        if (fechaCompra.isAfter(LocalDate.now())) {
            errores.add("fecha de compra: no puede ser a futuro.");
        }
    }

    // Valida que la cedula juridica tenga 10 digitos numericos sin letras
    private void validarCedulaJuridica(String cedulaJuridica, List<String> errores) {
        if (cedulaJuridica == null || !PATRON_CEDULA.matcher(cedulaJuridica).matches()) {
            errores.add("cedula juridica: debe tener exactamente 10 digitos numericos.");
        }
    }

    // Recorre todas las lineas y valida codigo de 10 digitos y cantidad positiva
    private void validarListaProductos(List<CompraDetalle> detalles, List<String> errores) {
        if (detalles == null || detalles.isEmpty()) {
            errores.add("La compra debe incluir al menos un producto.");
            return;
        }

        for (int i = 0; i < detalles.size(); i++) {
            CompraDetalle detalle = detalles.get(i);

            if (detalle.getNoProducto() == null || !PATRON_PRODUCTO.matcher(detalle.getNoProducto()).matches()) {
                errores.add("numero de producto en posicion " + (i + 1) + ": debe tener exactamente 10 digitos numericos.");
            }

            if (detalle.getCantidad() <= 0) {
                errores.add("cantidad del producto " + detalle.getNoProducto() + ": debe ser mayor a cero.");
            }
        }
    }
}