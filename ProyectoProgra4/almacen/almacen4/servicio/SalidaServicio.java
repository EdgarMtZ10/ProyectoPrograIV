package almacen.almacen4.servicio;

import almacen.almacen4.datos.Salida;
import almacen.almacen4.datos.SalidaDetalle;
import almacen.almacen4.datos.SalidaParser;
import almacen.almacen4.validaciones.SalidaValidador;
import almacen.almacen5.Bitacora;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.List;

/**
 * Procesa la trama de salida de inventario (ALMACEN4) contra SQL Server.
 */
public class SalidaServicio {

    private final SalidaParser parser;
    private final SalidaValidador validador;
    private final Connection conexion;

    // Constructor que recibe la conexion activa a SQL Server
    public SalidaServicio(Connection conexion) {
        this.parser = new SalidaParser();
        this.validador = new SalidaValidador();
        this.conexion = conexion;
    }

    // Parsea la trama, valida campos y descuenta inventario en una transaccion
    public String procesar(String tramaCruda) {
        SalidaParser.TramaSalida tramaParseada;

        // Si falla el parser devuelve DATO INVALIDO con el detalle exacto
        try {
            tramaParseada = parser.parsear(tramaCruda);
        } catch (IllegalArgumentException e) {
            return "DATO INVALIDO: " + e.getMessage();
        }

        Salida salida = tramaParseada.getSalida();

        // Si falla el validador informa todos los campos invalidos separados por ' | '
        List<String> errores = validador.validar(salida);
        if (!errores.isEmpty()) {
            return "DATO INVALIDO: " + String.join(" | ", errores);
        }

        String resultado;
        try {
            // Inicia transaccion manual para revertir si algun producto no tiene stock
            conexion.setAutoCommit(false);

            String resultadoCabecera = registrarCabecera(salida);
            if (!resultadoCabecera.equals("EXITOSO")) {
                conexion.rollback();
                resultado = resultadoCabecera;
                registrarBitacora(salida);
                return resultado;
            }

            String resultadoFinal = "EXITOSO";
            boolean huboError = false;

            // Registra cada producto de la salida
            for (SalidaDetalle detalle : salida.getDetalles()) {
                String resultadoDetalle = agregarDetalle(salida.getNumeroFactura(), detalle);

                if (!resultadoDetalle.equals("EXITOSO")) {
                    conexion.rollback();
                    resultadoFinal = resultadoDetalle;
                    huboError = true;
                    break;
                }
            }

            if (!huboError) {
                conexion.commit();
            }
            resultado = resultadoFinal;

        } catch (SQLException e) {
            try {
                conexion.rollback();
            } catch (SQLException rollbackEx) {
            }
            resultado = "ERROR";
        } finally {
            try {
                conexion.setAutoCommit(true);
            } catch (SQLException e) {
            }
        }

        registrarBitacora(salida);
        return resultado;
    }

    // Ejecuta sp_salida_registrar para insertar el encabezado de salida
    private String registrarCabecera(Salida salida) throws SQLException {
        String sql = "{call sp_salida_registrar(?, ?, ?)}";

        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            llamada.setLong(1, salida.getNumeroFactura());
            llamada.setDate(2, java.sql.Date.valueOf(salida.getFechaVenta()));

            llamada.registerOutParameter(3, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(3);
        }
    }

    // Ejecuta sp_salida_detalle_agregar para registrar la linea y bajar inventario
    private String agregarDetalle(long numeroFactura, SalidaDetalle detalle) throws SQLException {
        String sql = "{call sp_salida_detalle_agregar(?, ?, ?, ?)}";

        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            llamada.setLong(1, numeroFactura);
            llamada.setString(2, detalle.getNoProducto());
            llamada.setInt(3, detalle.getCantidad());

            llamada.registerOutParameter(4, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(4);
        }
    }

    // Guarda el movimiento estructurado en la bitacora JSON
    private void registrarBitacora(Salida salida) {
        StringBuilder productosJson = new StringBuilder("[");
        List<SalidaDetalle> detalles = salida.getDetalles();
        for (int i = 0; i < detalles.size(); i++) {
            SalidaDetalle d = detalles.get(i);
            productosJson.append(String.format(
                "{\"Numero producto\": \"%s\", \"Cantidad\": \"%d\"}",
                d.getNoProducto(), d.getCantidad()
            ));
            if (i < detalles.size() - 1) {
                productosJson.append(",");
            }
        }
        productosJson.append("]");

        String json = String.format(
            "{\"Transaccion\": \"6\", \"Numero Factura\": \"%s\", \"Fecha Venta\": \"%s\", \"Productos\": %s}",
            salida.getNumeroFactura(),
            salida.getFechaVenta(),
            productosJson
        );
        Bitacora.getInstancia().registrar(json);
    }
}