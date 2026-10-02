package almacen.socket;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

/**
 escucha en un puerto y, por cada conexion
 * nueva que llegue, lanzarla a un hilo aparte para que
 * la procese. Este hilo principal nunca lee ni procesa una trama el
 * mismo eso es justo lo que permite atender varias conexiones "al
 * mismo tiempo" sin que una bloquee a las demas 
 */
public class AlmacenServidor {

    private final int puerto;
    private final ConexionFactory conexionFactory;

    public AlmacenServidor(int puerto, ConexionFactory conexionFactory) {
        this.puerto = puerto;
        this.conexionFactory = conexionFactory;
    }

    /**
     * Arranca el servidor
     */
    public void iniciar() throws IOException {
        try (ServerSocket serverSocket = new ServerSocket(puerto)) {
            System.out.println("Almacen escuchando en el puerto " + puerto + "...");

            while (true) {
                // accept() se bloquea aqui hasta que llegue una conexion
                // nueva, en cuanto llega, se entrega un Socket ya
                // conectado a ese cliente especifico.
                Socket socketCliente = serverSocket.accept();

                // Se crea un hilo nuevo dedicado a este cliente y se
                // lanza de inmediato. El hilo principal vuelve al
                // accept() de la siguiente vuelta sin esperar a que
                // este cliente termine.
                ClienteHandler handler = new ClienteHandler(socketCliente, conexionFactory);
                Thread hiloCliente = new Thread(handler, "cliente-" + socketCliente.getPort());
                hiloCliente.start();
            }
        }
    }
}