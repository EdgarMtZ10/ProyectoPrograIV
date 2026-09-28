package almacen.almacen3.datos;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeParseException;
import java.util.ArrayList;
import java.util.List;

/**
 * Parser de la trama de texto plano
 *
 * Se encarga de transformar un String plano en un objeto Compra. 
 */
public class CompraParser {

    // ====================================================================
    // CONSTANTES  (Total 31 caracteres)
    // ====================================================================
    private static final int INICIO_TIPO_TRANSACCION = 1;
    private static final int TAMANO_TIPO_TRANSACCION = 1;

    private static final int INICIO_NUMERO_INGRESO = 2;
    private static final int TAMANO_NUMERO_INGRESO = 10;

    private static final int INICIO_FECHA = 12;
    private static final int TAMANO_FECHA = 10;

    private static final int INICIO_CEDULA_JURIDICA = 22;
    private static final int TAMANO_CEDULA_JURIDICA = 10;

    private static final int TAMANO_CABECERA = 31;

    
    // Constantes para el detalle (Productos)
    private static final int TAMANO_NO_PRODUCTO = 10;
    private static final int TAMANO_CANTIDAD = 7;
    private static final int TAMANO_BLOQUE_PRODUCTO = TAMANO_NO_PRODUCTO + TAMANO_CANTIDAD; // 17

    // Formateador de fecha para el patrón YYYYMMDD
    private static final DateTimeFormatter FORMATO_FECHA = DateTimeFormatter.ofPattern("yyyyMMdd");

    /**
     * Convierte la trama en un objeto TramaCompra.
     * 
     */
    public TramaCompra parsear(String trama) {
        // Validar que al menos venga la cabecera completa de 31 caracteres
        if (trama == null || trama.length() < TAMANO_CABECERA) {
            throw new IllegalArgumentException("Trama de compra demasiado corta. Faltan datos de cabecera.");
        }

        // Validar que la parte dinámica de los productos cuadre exactamente en bloques de 17
        int caracteresSobrantes = trama.length() - TAMANO_CABECERA;
        if (caracteresSobrantes % TAMANO_BLOQUE_PRODUCTO != 0) {
            throw new IllegalArgumentException("La lista de productos en la trama está incompleta o mal formada.");
        }

        // Extraer los datos de la cabecera
        String tipoTransaccion = extraerCampo(trama, INICIO_TIPO_TRANSACCION, TAMANO_TIPO_TRANSACCION);
        
        if (!tipoTransaccion.equals("5")) {
            throw new IllegalArgumentException("Tipo de transacción inválido. Se esperaba '5' (Compra).");
        }

        String numeroIngresoStr = extraerCampo(trama, INICIO_NUMERO_INGRESO, TAMANO_NUMERO_INGRESO);
        String fechaStr = extraerCampo(trama, INICIO_FECHA, TAMANO_FECHA);
        String cedulaJuridica = extraerCampo(trama, INICIO_CEDULA_JURIDICA, TAMANO_CEDULA_JURIDICA);

        // Conversión de tipos de cabecera
        long numeroIngreso;
        LocalDate fechaCompra;
        
        try {
            numeroIngreso = Long.parseLong(numeroIngresoStr); 
            // El trim() se encarga de quitar los 2 espacios sobrantes.
            fechaCompra = LocalDate.parse(fechaStr, FORMATO_FECHA);
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("El número de ingreso debe ser puramente numérico.");
        } catch (DateTimeParseException e) {
            throw new IllegalArgumentException("Formato de fecha inválido. Se esperaba YYYYMMDD.");
        }

        // Extraer la lista dinámica de productos
        List<CompraDetalle> detalles = new ArrayList<>();
        
        // Empezamos a leer justo donde termina la cabecera (índice 31)
        int indiceActual = TAMANO_CABECERA;
        
        while (indiceActual < trama.length()) {
            // Extraer el código del producto (siguientes 10 caracteres)
            String noProducto = trama.substring(indiceActual, indiceActual + TAMANO_NO_PRODUCTO).trim();
            indiceActual += TAMANO_NO_PRODUCTO;
            
            // Extraer la cantidad (siguientes 7 caracteres)
            String cantidadStr = trama.substring(indiceActual, indiceActual + TAMANO_CANTIDAD).trim();
            indiceActual += TAMANO_CANTIDAD;

            try {
                int cantidad = Integer.parseInt(cantidadStr);
                detalles.add(new CompraDetalle(noProducto, cantidad));
            } catch (NumberFormatException e) {
                throw new IllegalArgumentException("La cantidad del producto " + noProducto + " no es válida.");
            }
        }

        // Validar que la compra no venga sin productos
        if (detalles.isEmpty()) {
            throw new IllegalArgumentException("La trama no contiene ningún producto en la lista.");
        }

        //  Ensamblar y retornar el objeto final
        Compra compra = new Compra(numeroIngreso, fechaCompra, cedulaJuridica, detalles);
        return new TramaCompra(tipoTransaccion, compra);
    }

    /**
     * Utilidad para extraer campos fijos 
     */
    private String extraerCampo(String trama, int inicio1based, int tamano) {
        int inicio0based = inicio1based - 1;
        int fin0based = inicio0based + tamano;
        return trama.substring(inicio0based, fin0based).trim();
    }

    /**
     * Clase contenedora para transportar la transacción y la compra.
     */
    public static class TramaCompra {
        private final String tipoTransaccion;
        private final Compra compra;

        public TramaCompra(String tipoTransaccion, Compra compra) {
            this.tipoTransaccion = tipoTransaccion;
            this.compra = compra;
        }

        public String getTipoTransaccion() {
            return tipoTransaccion;
        }

        public Compra getCompra() {
            return compra;
        }
    }
}