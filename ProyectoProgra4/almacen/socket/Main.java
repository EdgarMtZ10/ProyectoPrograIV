package almacen.socket;

import java.io.IOException;

/**
 * Parte principal de la parte del Almacen.
 **/
public class Main {

    public static void main(String[] args) {
        // Se esperan 6 argumentos: puerto, hostSql, puertoSql, baseDatos, usuario, contrasena
        if (args.length != 6) {
            System.err.println("Uso: java almacen.socket.Main <puerto> <hostSql> <puertoSql> <baseDatos> <usuario> <contrasena>");
            return;
        }
        
        int puerto = Integer.parseInt(args[0]);
        String hostSql = args[1];
        int puertoSql = Integer.parseInt(args[2]);
        String baseDatos = args[3];
        String usuario = args[4];
        String contrasena = args[5];

        // Se crea la fabrica de conexiones y el servidor
        ConexionFactory conexionFactory = new ConexionFactory(hostSql, puertoSql, baseDatos, usuario, contrasena);
        AlmacenServidor servidor = new AlmacenServidor(puerto, conexionFactory);

        try {
            servidor.iniciar();
        } catch (IOException e) {
            System.err.println("No se pudo iniciar el servidor: " + e.getMessage());
        }
    }
}