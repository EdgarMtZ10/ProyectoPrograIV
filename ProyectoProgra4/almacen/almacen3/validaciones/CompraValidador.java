package almacen.almacen3.validaciones;

import almacen.almacen3.datos.Compra;
import almacen.almacen3.datos.CompraDetalle;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

/**
 * Valida las reglas de negocio de una compra antes de enviarla
 * a la base de datos.
 *
 * Aplica los criterios de validación de la siguiente manera:
 * - Número de ingreso: positivo.
 * - Fecha de compra: no puede ser a futuro.
 * - Cédula jurídica: 10 caracteres, sin letras.
 * - Productos: número de producto sin letras y positivo, cantidad positiva.
 */
public class CompraValidador {

    // La cédula jurídica debe tener exactamente 10 dígitos numéricos
    private static final Pattern PATRON_CEDULA = Pattern.compile("^\\d{10}$");//pattern para validar que la cédula jurídica tenga exactamente 10 dígitos numéricos
    
    // El número de producto debe tener exactamente 10 dígitos numéricos
    private static final Pattern PATRON_PRODUCTO = Pattern.compile("^\\d{10}$");//pattern para validar que el número de producto tenga exactamente 10 dígitos numéricos

    /**
     * Revisa todas las reglas de negocio sobre una Compra y devuelve la
     * lista de errores encontrados. Lista vacía significa que la
     * Compra es válida en su formato.
     */
    public List<String> validar(Compra compra) {
        List<String> errores = new ArrayList<>();//lista de errores encontrados durante la validación

        validarNumeroIngreso(compra.getNumeroIngreso(), errores);
        validarFechaCompra(compra.getFechaCompra(), errores);
        validarCedulaJuridica(compra.getCedulaJuridica(), errores);
        validarListaProductos(compra.getDetalles(), errores);

        return errores;
    }

    /**
     * Atajo para saber si la compra es válida sin extraer la lista de errores.
     */
    public boolean esValida(Compra compra) {
        return validar(compra).isEmpty();
    }

    /**
     * Valida que el número de ingreso sea un número válido y positivo.
     */
    private void validarNumeroIngreso(long numeroIngreso, List<String> errores) {
        if (numeroIngreso <= 0) {
            errores.add("El número de ingreso debe ser un valor positivo y mayor a cero.");
        }
    }

    /**
     * Valida que la fecha de compra no sea una fecha a futuro.
     */
    private void validarFechaCompra(LocalDate fechaCompra, List<String> errores) {//errores es la lista de errores encontrados durante la validación
        if (fechaCompra == null) {
            errores.add("La fecha de compra no puede ser nula.");
            return;
        }
        
        // Se compara la fecha de la trama con la fecha actual del sistema
        if (fechaCompra.isAfter(LocalDate.now())) {
            errores.add("La fecha de compra no puede ser a futuro.");
        }
    }

    /**
     * Valida que la cédula jurídica no contenga letras y sea del tamaño correcto.
     */
    private void validarCedulaJuridica(String cedulaJuridica, List<String> errores) {
        if (cedulaJuridica == null || !PATRON_CEDULA.matcher(cedulaJuridica).matches()) {
            errores.add("La cédula jurídica debe tener exactamente 10 dígitos numéricos, sin letras.");
        }
    }

    /**
     * Recorre la lista de productos y valida que el código no tenga letras y 
     * que la cantidad sea un número positivo.
     */
    private void validarListaProductos(List<CompraDetalle> detalles, List<String> errores) {
        if (detalles == null || detalles.isEmpty()) {
            errores.add("La compra debe incluir al menos un producto.");
            return;
        }
        //array de detalles de la compra, se recorre para validar cada producto
        for (int i = 0; i < detalles.size(); i++) {
            CompraDetalle detalle = detalles.get(i);
            
            // Validar código de producto (sin letras, solo números)
            if (detalle.getNoProducto() == null || !PATRON_PRODUCTO.matcher(detalle.getNoProducto()).matches()) {
                errores.add("El número de producto en la posición " + (i + 1) + " es inválido. Debe ser de 10 dígitos numéricos.");
            }
            
            // Validar cantidad (positivo)
            if (detalle.getCantidad() <= 0) {
                errores.add("La cantidad del producto '" + detalle.getNoProducto() + "' debe ser mayor a cero.");
            }
        }
    }
}