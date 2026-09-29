package almacen.almacen1.datos;

import java.math.BigDecimal;

/**
 * Representa un producto extraido de la trama de ALMACEN1.
 */
public class Producto {

    private final String noProducto;
    private final String nombre;
    private final BigDecimal precio;

    public Producto(String noProducto, String nombre, BigDecimal precio) {
        this.noProducto = noProducto;
        this.nombre = nombre;
        this.precio = precio;
    }

    public String getNoProducto() {
        return noProducto;
    }

    public String getNombre() {
        return nombre;
    }

    public BigDecimal getPrecio() {
        return precio;
    }
}