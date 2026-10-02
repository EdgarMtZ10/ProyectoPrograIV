package almacen.bitacora;

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
 * Formato de cada linea: dd/MM/yyyy HH:mm:ss: {json}
 */
public class Bitacora {

    private static final String RUTA_ARCHIVO = "bitacora_almacen.txt";
    private static final DateTimeFormatter FORMATO_FECHA =
            DateTimeFormatter.ofPattern("dd/MM/yyyy HH:mm:ss");

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
        String fecha = LocalDateTime.now().format(FORMATO_FECHA);
        String linea = fecha + ": " + jsonDetalle;
        cola.offer(linea);
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