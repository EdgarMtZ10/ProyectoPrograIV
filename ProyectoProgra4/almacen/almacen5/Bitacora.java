package almacen.almacen5;

import java.io.FileWriter;
import java.io.IOException;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

/**
 * Bitacora de operaciones del Almacen (ALMACEN5).
 *
 * Escribe en segundo plano, en un hilo independiente, sin bloquear
 * la ejecucion de los servicios. Las solicitudes de escritura se
 * encolan para acceder al archivo de forma ordenada.
 *
 * Formato de cada linea: una linea JSON con la fecha como primer dato
 * {"fecha": "yyyy-MM-dd HH:mm:ss", ...detalle de la transaccion...}
 */
public class Bitacora {

    private static final String RUTA_ARCHIVO = "bitacora_almacen.txt";
    private static final DateTimeFormatter FORMATO_FECHA =
            DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

    private static final Bitacora INSTANCIA = new Bitacora();

    private final BlockingQueue<String> cola = new LinkedBlockingQueue<>();
    private final Thread hiloEscritor;

    private Bitacora() {
        hiloEscritor = new Thread(this::procesarCola, "hilo-bitacora");
        hiloEscritor.setDaemon(true);
        hiloEscritor.start();
    }

    /**
     * Punto de acceso unico para todos los servicios del almacen.
     */
    public static Bitacora getInstancia() {
        return INSTANCIA;
    }

    /*
      Encola una transaccion para ser escrita en la bitacora
      No bloquea: el servicio que llama sigue su ejecucion normal
     
      @param jsonDetalle el detalle de la transaccion ya armado en JSON
                         por el servicio correspondiente (ej. producto,
                        proveedor, compra o salida)
     */
    public void registrar(String jsonDetalle) {
        String fecha = "\"fecha\": \"" + LocalDateTime.now().format(FORMATO_FECHA) + "\"";
        String detalle = (jsonDetalle == null) ? "" : jsonDetalle.trim();

        // la fecha se coloca como primer dato dentro del mismo objeto JSON
        String linea;
        if (detalle.startsWith("{") && detalle.endsWith("}")) {
            String contenido = detalle.substring(1, detalle.length() - 1).trim();
            linea = contenido.isEmpty() ? "{" + fecha + "}" : "{" + fecha + ", " + contenido + "}";
        } else {
            // si el detalle no viene en JSON se guarda como texto
            linea = "{" + fecha + ", \"detalle\": \"" + escapar(detalle) + "\"}";
        }
        cola.offer(linea);
    }

    /*
      Registra la trama de respuesta (movimiento de salida) que el almacen
      devuelve por el socket para la transaccion indicada.
     */
    public void registrarRespuesta(String tipoTransaccion, String tramaRespuesta) {
        registrar("{\"Movimiento\": \"salida\", \"Transaccion\": \"" + escapar(tipoTransaccion)
                + "\", \"Trama respuesta\": \"" + escapar(tramaRespuesta) + "\"}");
    }

    // escapa barras y comillas para no romper el JSON
    public static String escapar(String texto) {
        return (texto == null) ? "" : texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }

    /*
      Corre en el hilo independiente. Procesa la cola en orden
      y escribe cada linea en el archivo de bitacora.
     */
    private void procesarCola() {
        while (true) {
            try {
                String linea = cola.take(); // Espera si la cola esta vacia
                escribirLinea(linea);
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                break;
            }
        }
    }

    private void escribirLinea(String linea) {
        try (FileWriter escritor = new FileWriter(RUTA_ARCHIVO, true)) {
            escritor.write(linea);
            escritor.write(System.lineSeparator());
        } catch (IOException e) {
            System.err.println("Error al escribir en la bitacora: " + e.getMessage());
        }
    }
}