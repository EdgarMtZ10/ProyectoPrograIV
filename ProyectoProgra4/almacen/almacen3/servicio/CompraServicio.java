package almacen.almacen3.servicio;

import almacen.almacen3.datos.Compra;
import almacen.almacen3.datos.CompraDetalle;
import almacen.almacen3.datos.CompraParser;
import almacen.almacen3.validaciones.CompraValidador;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.List;

import almacen.bitacora.Bitacora;

/**
 * Servicio de Compras del almacen 3.
 *
 * Hace el flujo completo de una compra:
 * 1. Extrae los datos de la trama dinámica.
 * 2. Valida las reglas de negocio.
 * 3. Ejecuta la transacción en la base de datos.
 * 4. Devuelve el texto exacto de respuesta.
 */
public class CompraServicio {

    private final CompraParser parser;
    private final CompraValidador validador;
    private final Connection conexion;

    // Constructor que recibe la conexión a la base de datos
    public CompraServicio(Connection conexion) {
        this.parser = new CompraParser();
        this.validador = new CompraValidador();
        this.conexion = conexion;
    }

    /**
     * Procesa la trama completa y maneja la transacción de base de datos.
     */
    public String procesar(String tramaCruda) {
        CompraParser.TramaCompra tramaParseada;

        //bloque de validacion
        // Parser de la trama (Si la longitud o formato base falla, es DATO INVÁLIDO)
        try {
            tramaParseada = parser.parsear(tramaCruda);
        } catch (IllegalArgumentException e) {
            return "DATO INVALIDO";
        }

        Compra compra = tramaParseada.getCompra();// Extrae el objeto Compra del resultado del parser

        //Validación de reglas de negocio (sin tocar BD)
        List<String> errores = validador.validar(compra);
        if (!errores.isEmpty()) {
            return "DATO INVALIDO";
        }

        // Ejecución en base de datos con manejo de Transacciones
                String resultado;
        try {
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

    /**
     * Llama a sp_compra_registrar para guardar el encabezado.
     */
    private String registrarCabecera(Compra compra) throws SQLException {
        String sql = "{call sp_compra_registrar(?, ?, ?, ?)}";//consulta a procedimiento almacenado

        try (CallableStatement llamada = conexion.prepareCall(sql)) {//apertura de la llamada al procedimiento almacenado
            llamada.setLong(1, compra.getNumeroIngreso());
            // Convertimos el LocalDate a java.sql.Date para SQL Server
            //parametros de entrada
            llamada.setDate(2, java.sql.Date.valueOf(compra.getFechaCompra()));
            llamada.setString(3, compra.getCedulaJuridica());
            
            //parametros de salida
            llamada.registerOutParameter(4, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(4);
        }
    }

    /**
     * Llama a sp_compra_detalle_agregar para guardar un producto.
     * El trigger en SQL Server se encargará de subir el inventario automáticamente.
     */
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