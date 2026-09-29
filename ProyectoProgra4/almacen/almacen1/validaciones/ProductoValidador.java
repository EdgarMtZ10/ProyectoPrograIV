
package almacen.almacen1.validaciones;

import almacen.almacen1.datos.Producto;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

/**
 * Valida las reglas de negocio de un producto antes de enviarlo
 * a la base de datos.
 *
 * Aplica los criterios de validacion de la siguiente manera:
 * - Numero de producto: sin letras, positivo (10 digitos).
 * - Nombre: no puede ser puros numericos ni estar vacio o en blanco.
 * - Precio: debe ser un valor positivo.
 */
public class ProductoValidador {

    // El numero de producto debe tener exactamente 10 digitos numericos
    private static final Pattern PATRON_PRODUCTO = Pattern.compile("^\\d{10}$");

    // Un nombre "puramente numerico" es invalido (debe contener al menos una letra)
    private static final Pattern PATRON_SOLO_NUMEROS = Pattern.compile("^\\d+$");

    /**
     * Revisa todas las reglas de negocio sobre un Producto y devuelve la
     * lista de errores encontrados. Lista vacia significa que el
     * Producto es valido en su formato.
     */
    public List<String> validar(Producto producto) {
        List<String> errores = new ArrayList<>();

        validarNoProducto(producto.getNoProducto(), errores);
        validarNombre(producto.getNombre(), errores);
        validarPrecio(producto.getPrecio(), errores);

        return errores;
    }

    /**
     * Atajo para saber si el producto es valido sin extraer la lista de errores.
     */
    public boolean esValido(Producto producto) {
        return validar(producto).isEmpty();
    }

    /**
     * Valida que el numero de producto no contenga letras y sea del tamano correcto.
     */
    private void validarNoProducto(String noProducto, List<String> errores) {
        if (noProducto == null || !PATRON_PRODUCTO.matcher(noProducto).matches()) {
            errores.add("El numero de producto debe tener exactamente 10 digitos numericos, sin letras.");
        }
    }

    /**
     * Valida que el nombre no sea puros numericos ni este en blanco o vacio.
     */
    private void validarNombre(String nombre, List<String> errores) {
        if (nombre == null || nombre.trim().isEmpty()) {
            errores.add("El nombre del producto no puede estar vacio o en blanco.");
            return;
        }

        if (PATRON_SOLO_NUMEROS.matcher(nombre.trim()).matches()) {
            errores.add("El nombre del producto no puede ser puramente numerico.");
        }
    }

    /**
     * Valida que el precio sea un valor positivo.
     */
    private void validarPrecio(BigDecimal precio, List<String> errores) {
        if (precio == null || precio.compareTo(BigDecimal.ZERO) <= 0) {
            errores.add("El precio debe ser un valor positivo.");
        }
    }
}