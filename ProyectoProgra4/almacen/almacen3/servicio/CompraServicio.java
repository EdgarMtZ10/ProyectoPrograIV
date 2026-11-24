package almacen.almacen3.servicio;

import almacen.almacen3.datos.Compra;
import almacen.almacen3.datos.CompraDetalle;
import almacen.almacen3.datos.CompraParser;
import almacen.almacen3.validaciones.CompraValidador;
import almacen.almacen5.Bitacora;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.List;

/**
 * Procesa la trama de compra (ALMACEN3), valida reglas y ejecuta la transaccion en SQL Server.
 */
public class CompraServicio {

    private final CompraParser parser;
    private final CompraValidador validador;
    private final Connection conexion;

    // Constructor que recibe la conexion activa a SQL Server
    public CompraServicio(Connection conexion) {
        this.parser = new CompraParser();
        this.validador = new CompraValidador();
        this.conexion = conexion;
    }

    // Parsea la trama, valida campos y ejecuta cabecera + lineas dentro de una transaccion
    public String procesar(String tramaCruda) {
        CompraParser.TramaCompra tramaParseada;

        // Si falla el parser devuelve DATO INVALIDO con el detalle exacto
        try {
            tramaParseada = parser.parsear(tramaCruda);
        } catch (IllegalArgumentException e) {
            return "DATO INVALIDO: " + e.getMessage();
        }

        Compra compra = tramaParseada.getCompra();

        // Si falla el validador informa todos los campos invalidos separados por ' | '
        List<String> errores = validador.validar(compra);
        if (!errores.isEmpty()) {
            return "DATO INVALIDO: " + String.join(" | ", errores);
        }

        String resultado;
        try {
            // Inicia transaccion manual para revertir si falla alguna linea
            conexion.setAutoCommit(false);

            String resultadoCabecera = registrarCabecera(compra);
            if (!resultadoCabecera.equals("EXITOSO")) {
                conexion.rollback();
                resultado = resultadoCabecera;
                registrarBitacora(compra);
                return resultado;
            }

            String resultadoFinal = "EXITOSO";
            boolean huboError = false;

            // Registra cada producto de la lista de compra
            for (CompraDetalle detalle : compra.getDetalles()) {
                String resultadoDetalle = agregarDetalle(compra.getNumeroIngreso(), detalle);

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

        registrarBitacora(compra);
        return resultado;
    }

    // Ejecuta sp_compra_registrar para insertar el encabezado
    private String registrarCabecera(Compra compra) throws SQLException {
        String sql = "{call sp_compra_registrar(?, ?, ?, ?)}";

        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            llamada.setLong(1, compra.getNumeroIngreso());
            llamada.setDate(2, java.sql.Date.valueOf(compra.getFechaCompra()));
            llamada.setString(3, compra.getCedulaJuridica());

            llamada.registerOutParameter(4, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(4);
        }
    }

    // Ejecuta sp_compra_detalle_agregar para insertar una linea de producto
    private String agregarDetalle(long numeroIngreso, CompraDetalle detalle) throws SQLException {
        String sql = "{call sp_compra_detalle_agregar(?, ?, ?, ?)}";

        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            llamada.setLong(1, numeroIngreso);
            llamada.setString(2, detalle.getNoProducto());
            llamada.setInt(3, detalle.getCantidad());

            llamada.registerOutParameter(4, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(4);
        }
    }

    // Guarda el movimiento estructurado en la bitacora JSON
    private void registrarBitacora(Compra compra) {
        StringBuilder productosJson = new StringBuilder("[");
        List<CompraDetalle> detalles = compra.getDetalles();
        for (int i = 0; i < detalles.size(); i++) {
            CompraDetalle d = detalles.get(i);
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
            "{\"Transaccion\": \"5\", \"Numero Ingreso\": \"%s\", \"Fecha compra\": \"%s\", " +
            "\"Cedula Juridica\": \"%s\", \"Productos\": %s}",
            compra.getNumeroIngreso(),
            compra.getFechaCompra(),
            compra.getCedulaJuridica(),
            productosJson
        );
        Bitacora.getInstancia().registrar(json);
    }
}