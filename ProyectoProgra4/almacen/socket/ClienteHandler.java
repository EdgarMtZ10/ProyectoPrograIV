package almacen.socket;

import almacen.almacen1.servicio.ProductoServicio;
import almacen.almacen2.servicio.ProveedorServicio;
import almacen.almacen3.servicio.CompraServicio;
import almacen.almacen4.servicio.SalidaServicio;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.SQLException;

/**
 * CLienteHandler porque es el encargado de manejar la conexion con un cliente especifico.
 * Atiende una conexion de un cliente, de principio a fin, en su propio
 * hilo.
 * Implementa Runnable  para que AlmacenServidor sea
 * capaz de lanzar un hilo nuevo para cada cliente que llegue.
 */
public class ClienteHandler implements Runnable {

    // El socket que representa la conexion con el cliente
    private final Socket socketCliente;
    private final ConexionFactory conexionFactory;

    // Constructor
    public ClienteHandler(Socket socketCliente, ConexionFactory conexionFactory) {
        this.socketCliente = socketCliente;
        this.conexionFactory = conexionFactory;
    }

    @Override
    public void run() {
        // se garantiza que el socket Y la conexion a
        // la base de datos se cierren siempre, incluso si algo lanza
        // una excepcion a mitad de camino.
        try (
            Socket socket = this.socketCliente;
            BufferedReader entrada = new BufferedReader(
                new InputStreamReader(socket.getInputStream(), StandardCharsets.UTF_8));
            PrintWriter salida = new PrintWriter(socket.getOutputStream(), true);
            Connection conexion = conexionFactory.nuevaConexion()
        ) {
            // asume que el simulador manda la trama seguida
            // de un salto de linea. 
            String tramaCruda = entrada.readLine();

            String respuesta = despachar(tramaCruda, conexion);

            salida.println(respuesta);

        } catch (SQLException e) {
            // Fallo al conectar con SQL Server
            System.err.println("Error de base de datos: " + e.getMessage());
        } catch (IOException e) {
            // Fallo de red - No hay a quien responderle.
            System.err.println("Error de conexion con el cliente: " + e.getMessage());
        }
    }

    /**
     *Mira el primer caracter de la trama (el tipo de
     * transaccion) y decide a cual servicio pertenece.
     *
      cada servicio recibe la trama cruda completa, no
     * solo lo que sigue despues del primer caracter
     */
    private String despachar(String tramaCruda, Connection conexion) {
        if (tramaCruda == null || tramaCruda.isEmpty()) {
            return "DATO INVALIDO";
        }

        //char porque el primer caracter de la trama indica el tipo de transaccion
        char tipoTransaccion = tramaCruda.charAt(0);

        //switch para despachar a cada servicio segun el tipo de transaccion porque cada servicio tiene su propia logica de negocio y su propia forma de procesar la trama
        switch (tipoTransaccion) {
            case '1':
            case '2':
                return new ProductoServicio(conexion).procesar(tramaCruda);

            case '3':
            case '4':
                return new ProveedorServicio(conexion).procesar(tramaCruda);

            case '5':
                return new CompraServicio(conexion).procesar(tramaCruda);

            case '6':
                return new SalidaServicio(conexion).procesar(tramaCruda);

            default:
                return "DATO INVALIDO";
        }
    }
}