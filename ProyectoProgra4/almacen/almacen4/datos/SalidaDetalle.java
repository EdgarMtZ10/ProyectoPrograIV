package almacen.almacen4.datos;

/**
 * Representa una linea de la lista de productos de una salida en ALMACEN4.
 *Igual que en Almacen 3, solo guarda el codigo de
 * producto y la cantidad a retirar del inventario.
 */
public class SalidaDetalle {

    //
    private final String noProducto;
    private final int cantidad;

    public SalidaDetalle(String noProducto, int cantidad) {
        this.noProducto = noProducto;
        this.cantidad = cantidad;
    }

    public String getNoProducto() {
        return noProducto;
    }

    public int getCantidad() {
        return cantidad;
    }
}