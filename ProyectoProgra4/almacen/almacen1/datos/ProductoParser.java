package almacen.almacen1.datos;

import java.math.BigDecimal;

/*
 * Parser de la trama de texto plano para Producto (ALMACEN1).
 
  Se encarga de transformar un String plano en un objeto Producto.
 */
public class ProductoParser {

    
    // CONSTANTES  (Total 109 caracteres, trama fija sin lista)
    
    private static final int INICIO_TIPO_TRANSACCION = 1;
    private static final int TAMANO_TIPO_TRANSACCION = 1;

    private static final int INICIO_NO_PRODUCTO = 2;
    private static final int TAMANO_NO_PRODUCTO = 10;

    private static final int INICIO_NOMBRE = 12;
    private static final int TAMANO_NOMBRE = 90;

    private static final int INICIO_PRECIO = 102;
    private static final int TAMANO_PRECIO = 8;

    private static final int TAMANO_TRAMA = 109;

    /*
      Convierte la trama en un objeto TramaProducto
     */
    public TramaProducto parsear(String trama) {
        if (trama == null || trama.length() != TAMANO_TRAMA) {
            throw new IllegalArgumentException(
                "Trama de producto con longitud invalida. Se esperaban " + TAMANO_TRAMA + " caracteres.");
        }

        String tipoTransaccion = extraerCampo(trama, INICIO_TIPO_TRANSACCION, TAMANO_TIPO_TRANSACCION);

        if (!tipoTransaccion.equals("1") && !tipoTransaccion.equals("2")) {
            throw new IllegalArgumentException("Tipo de transaccion invalido. Se esperaba '1' (Ingreso) o '2' (Modificacion).");
        }

        String noProducto = extraerCampo(trama, INICIO_NO_PRODUCTO, TAMANO_NO_PRODUCTO);
        String nombre = extraerCampo(trama, INICIO_NOMBRE, TAMANO_NOMBRE);
        String precioStr = extraerCampo(trama, INICIO_PRECIO, TAMANO_PRECIO);

        BigDecimal precio;
        try {
            // Los ultimos 2 digitos son decimales
            BigDecimal valorEntero = new BigDecimal(precioStr);
            precio = valorEntero.movePointLeft(2);
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("El precio debe ser puramente numerico.");
        }

        Producto producto = new Producto(noProducto, nombre, precio);
        return new TramaProducto(tipoTransaccion, producto);
    }

    /*
      Utilidad para extraer campos fijos
     */
    private String extraerCampo(String trama, int inicio1based, int tamano) {
        int inicio0based = inicio1based - 1;
        int fin0based = inicio0based + tamano;
        return trama.substring(inicio0based, fin0based).trim();
    }

    /*
      Clase contenedora para transportar la transaccion y el producto.
     */
    public static class TramaProducto {
        private final String tipoTransaccion;
        private final Producto producto;

        public TramaProducto(String tipoTransaccion, Producto producto) {
            this.tipoTransaccion = tipoTransaccion;
            this.producto = producto;
        }

        public String getTipoTransaccion() {
            return tipoTransaccion;
        }

        public Producto getProducto() {
            return producto;
        }
    }
}