package almacen.almacen2.servicio;

import almacen.almacen2.datos.Proveedor;
import almacen.almacen2.datos.ProveedorParser;
import almacen.almacen2.datos.TipoTransaccionProveedor;
import almacen.almacen2.validaciones.ProveedorValidador;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.List;

import almacen.almacen5.Bitacora;

/**
 *   1. Recibe la trama de 270 caracteres desde el socket.
 *   2. obtiene un Proveedor + el tipo
 *      de transaccion.
 *   3. Valida el FORMATO con ProveedorValidador. Si
 *      falla, responde "DATO INVALIDO" de una vez, sin gastar una
 *      consulta a SQL Server.
 *   4. Si el formato esta bien, llama al prodedimiento almacenado
 *      sp_proveedor_mantenimiento y hace el
 *      INSERT o UPDATE.
 *   5. Devuelve el texto que el socket debe mandar de vuelta en
 *      la trama de respuesta (EXITOSO, DUPLICADO, DATO INVALIDO,
 *      PROVEEDOR INVALIDO o ERROR).
 */
public class ProveedorServicio {

    private final ProveedorParser parser;
    private final ProveedorValidador validador;
    private final Connection conexion;

    public ProveedorServicio(Connection conexion) {
        this.parser = new ProveedorParser();
        this.validador = new ProveedorValidador();
        this.conexion = conexion;
    }

    /**
     * Procesa una trama completa de ALMACEN2 y devuelve el texto de
     * respuesta que el socket debe enviar de vuelta.
     */
    public String procesar(String tramaCruda) {
        ProveedorParser.TramaProveedor tramaParseada;

        // Si la trama viene con un largo incorrecto o
        // con un tipo de transaccion / estado invalido, el parser lanza
        // IllegalArgumentException - se atrapa aqui y se responde
        // DATO INVALIDO.
        try {
            tramaParseada = parser.parsear(tramaCruda);
        } catch (IllegalArgumentException e) {
            return "DATO INVALIDO";
        }

        Proveedor proveedor = tramaParseada.getProveedor();
        TipoTransaccionProveedor tipoTransaccion = tramaParseada.getTipoTransaccion();

        //validar formato (sin tocar la base de datos).
        List<String> errores = validador.validar(proveedor);
        if (!errores.isEmpty()) {
            // se registrara el detalle de errores antes de
            // responder.
            return "DATO INVALIDO";
        }

        // el formato ya esta validado, ahora se delega a la base
        // de datos para revisar duplicados
        // (Ingreso) o existencia (Modificacion).
                String resultado;
        try {
            resultado = ejecutarStoredProcedure(tipoTransaccion, proveedor);
        } catch (SQLException e) {
            resultado = "ERROR";
        }

        registrarBitacora(tipoTransaccion.getCodigoTrama(), proveedor);
        return resultado;
    }

    /**
     * Llama al procedimiento almacenado sp_proveedor_mantenimiento y traduce su
     * parametro de salida
     */
    private String ejecutarStoredProcedure(TipoTransaccionProveedor tipoTransaccion,
                                            Proveedor proveedor) throws SQLException {
        String sql = "{call sp_proveedor_mantenimiento(?, ?, ?, ?, ?, ?, ?, ?)}";

        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            // Parametros de entrada, en el mismo orden que el CREATE
            // PROCEDURE de la base de datos.
            llamada.setString(1, tipoTransaccion.getCodigoTrama());
            llamada.setString(2, proveedor.getCedulaJuridica());
            llamada.setString(3, proveedor.getNombre());
            llamada.setString(4, proveedor.getNombreContacto());
            llamada.setString(5, proveedor.getTelefono());
            llamada.setString(6, proveedor.getCorreo());
            llamada.setString(7, proveedor.getEstado().getCodigoTrama());

            // Parametro de salida: aqui es donde el procedimiento almacenado deja
            // escrito EXITOSO / DUPLICADO / PROVEEDOR INVALIDO / etc.
            llamada.registerOutParameter(8, Types.VARCHAR);

            llamada.execute();

            return llamada.getString(8);
        }
    }
        //finalmente, se registra la transaccion en la bitacora.
        private void registrarBitacora(String tipoTransaccion, Proveedor proveedor) {
        String json = String.format(
            // Se escapan las comillas dobles de los nombres y se registra el estado como su codigo de trama.
            "{\"Transaccion\": \"%s\", \"Cedula Juridica\": \"%s\", \"Nombre\": \"%s\", " +
            "\"Nombre Contacto\": \"%s\", \"Telefono\": \"%s\", \"Correo\": \"%s\", \"Estado\": \"%s\"}",
            tipoTransaccion,
            proveedor.getCedulaJuridica(),
            proveedor.getNombre().replace("\"", "\\\""),
            proveedor.getNombreContacto().replace("\"", "\\\""),
            proveedor.getTelefono(),
            proveedor.getCorreo(),
            proveedor.getEstado().getCodigoTrama()
        );
        Bitacora.getInstancia().registrar(json);
    }
}
