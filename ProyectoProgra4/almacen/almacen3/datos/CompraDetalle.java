package almacen.almacen3.datos;

public class CompraDetalle {
    private final String noProducto;//constante de tipo String que almacena el número de producto
    private final int cantidad;//constante de tipo int que almacena la cantidad de producto

    public CompraDetalle(String noProducto, int cantidad) {
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