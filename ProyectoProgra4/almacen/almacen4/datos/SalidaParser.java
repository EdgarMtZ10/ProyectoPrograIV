package almacen.almacen4.datos;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeParseException;
import java.util.ArrayList;
import java.util.List;

/**
 * Parser para la trama de ALMACEN4 
 */
public class SalidaParser {

    // ==================================================================
    // Constanes (cabecera = 21 caracteres)
    // ==================================================================
    private static final int INICIO_TIPO_TRANSACCION = 1;
    private static final int TAMANO_TIPO_TRANSACCION = 1;

    private static final int INICIO_NUMERO_FACTURA = 2;
    private static final int TAMANO_NUMERO_FACTURA = 10;

    private static final int INICIO_FECHA = 12;
    private static final int TAMANO_FECHA = 10;

    private static final int TAMANO_CABECERA = 21;

    // Constantes del detalle 17 por producto
    private static final int TAMANO_NO_PRODUCTO = 10;
    private static final int TAMANO_CANTIDAD = 7;
    private static final int TAMANO_BLOQUE_PRODUCTO = TAMANO_NO_PRODUCTO + TAMANO_CANTIDAD; // 17

    private static final DateTimeFormatter FORMATO_FECHA = DateTimeFormatter.ofPattern("yyyyMMdd");

    /**
     * Convierte la trama en un TramaSalida.
     */
    public TramaSalida parsear(String trama) {
        // Validar que al menos venga la cabecera completa de 21 caracteres
        if (trama == null || trama.length() < TAMANO_CABECERA) {
            throw new IllegalArgumentException("Trama de salida demasiado corta. Faltan datos de cabecera.");
        }

        // Validar que la parte de productos cuadre exactamente en bloques de 17
        int caracteresSobrantes = trama.length() - TAMANO_CABECERA;
        if (caracteresSobrantes % TAMANO_BLOQUE_PRODUCTO != 0) {
            throw new IllegalArgumentException("La lista de productos en la trama esta incompleta o mal formada.");
        }

        String tipoTransaccion = extraerCampo(trama, INICIO_TIPO_TRANSACCION, TAMANO_TIPO_TRANSACCION);

        if (!tipoTransaccion.equals("6")) {
            throw new IllegalArgumentException("Tipo de transaccion invalido. Se esperaba '6' (Salida).");
        }

        String numeroFacturaStr = extraerCampo(trama, INICIO_NUMERO_FACTURA, TAMANO_NUMERO_FACTURA);
        String fechaStr = extraerCampo(trama, INICIO_FECHA, TAMANO_FECHA);

        long numeroFactura;
        LocalDate fechaVenta;

        try {
            numeroFactura = Long.parseLong(numeroFacturaStr);
            fechaVenta = LocalDate.parse(fechaStr, FORMATO_FECHA);
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("El numero de factura debe ser puramente numerico.");
        } catch (DateTimeParseException e) {
            throw new IllegalArgumentException("Formato de fecha invalido. Se esperaba YYYYMMDD.");
        }

        // Extraer la lista dinamica de productos
        List<SalidaDetalle> detalles = new ArrayList<>();
        int indiceActual = TAMANO_CABECERA;

        while (indiceActual < trama.length()) {
            String noProducto = trama.substring(indiceActual, indiceActual + TAMANO_NO_PRODUCTO).trim();
            indiceActual += TAMANO_NO_PRODUCTO;

            String cantidadStr = trama.substring(indiceActual, indiceActual + TAMANO_CANTIDAD).trim();
            indiceActual += TAMANO_CANTIDAD;

            try {
                int cantidad = Integer.parseInt(cantidadStr);
                detalles.add(new SalidaDetalle(noProducto, cantidad));
            } catch (NumberFormatException e) {
                throw new IllegalArgumentException("La cantidad del producto " + noProducto + " no es valida.");
            }
        }

        if (detalles.isEmpty()) {
            throw new IllegalArgumentException("La trama no contiene ningun producto en la lista.");
        }

        Salida salida = new Salida(numeroFactura, fechaVenta, detalles);
        return new TramaSalida(tipoTransaccion, salida);
    }

    private String extraerCampo(String trama, int inicio1based, int tamano) {
        int inicio0based = inicio1based - 1;//Suma la longitud requerida a la posición inicial para determinar el punto de corte.
        int fin0based = inicio0based + tamano;
        return trama.substring(inicio0based, fin0based).trim();// extrae el texto desde el índice inicial hasta un carácter antes del índice final
    }

    /**
     * Clase contenedora para transportar la transaccion y la salida.
     */
    public static class TramaSalida {
        private final String tipoTransaccion;
        private final Salida salida;

        public TramaSalida(String tipoTransaccion, Salida salida) {
            this.tipoTransaccion = tipoTransaccion;
            this.salida = salida;
        }

        public String getTipoTransaccion() {
            return tipoTransaccion;
        }

        public Salida getSalida() {
            return salida;
        }
    }
}