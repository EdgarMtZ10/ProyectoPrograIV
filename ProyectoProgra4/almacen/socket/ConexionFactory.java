package almacen.socket;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

/**
 *El unico responsable de abrir conexiones a SQL Server.
 *
 */
public class ConexionFactory {

    private final String urlConexion;
    private final String usuario;
    private final String contrasena;


    public ConexionFactory(String host, int puertoSql, String baseDatos,
                            String usuario, String contrasena) {
        // Trust server certificate = true es para desarrollo local.
        this.urlConexion = String.format(
            "jdbc:sqlserver://%s:%d;databaseName=%s;encrypt=true;trustServerCertificate=true",
            host, puertoSql, baseDatos
        );
        this.usuario = usuario;
        this.contrasena = contrasena;
    }

    /**
     * Abre una conexion nueva. El responsable de cerrarla
     * (ClienteHandler lo hace en su try-with-resources).
     */
    public Connection nuevaConexion() throws SQLException {
        return DriverManager.getConnection(urlConexion, usuario, contrasena);
    }
}