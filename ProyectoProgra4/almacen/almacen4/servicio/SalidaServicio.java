package almacen.almacen4.servicio;

import almacen.almacen4.datos.Salida;
import almacen.almacen4.datos.SalidaDetalle;
import almacen.almacen4.datos.SalidaParser;
import almacen.almacen4.validaciones.SalidaValidador;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.List;

import almacen.almacen5.Bitacora;

/**
    * Flujo de procesamiento:
    * 1. Extrae los datos de la trama dinámica.
    * 2. Valida las reglas de negocio.
    * 3. Ejecuta la transacción en la base de datos.
    * 4. Devuelve el texto exacto de respuesta. 
    * 5. Registra la transacción en la bitácora en formato JSON.
 **/
public class SalidaServicio {

    private final SalidaParser parser;
    private final SalidaValidador validador;
    private final Connection conexion;

    public SalidaServicio(Connection conexion) {
        this.parser = new SalidaParser();
        this.validador = new SalidaValidador();
        this.conexion = conexion;
    }

    public String procesar(String tramaCruda) {
        SalidaParser.TramaSalida tramaParseada;
        //recibe el texto original (tramaCruda) y declara una variable para guardar el resultado estructurado.

        /*Intenta convertir el texto crudo en un objeto Java. Si el parser encuentra un formato irreconocible
        el parser encuentra un formato irreconocible, lanza un IllegalArgumentException.
         Se atrapa y devuelve directamente "DATO INVALIDO". */
        try {
            tramaParseada = parser.parsear(tramaCruda);
        } catch (IllegalArgumentException e) {
            return "DATO INVALIDO";
        }

        Salida salida = tramaParseada.getSalida();

        //muestra los errores de validación de reglas de negocio, si los hay. Si hay errores, devuelve "DATO INVALIDO".
        List<String> errores = validador.validar(salida);
        if (!errores.isEmpty()) {
            return "DATO INVALIDO";
        }

                String resultado;
        try {
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

            // Registrar cada detalle de la salida
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
    
    //Metodos de acceso a BD
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
    
    //agrega un detalle de salida a la base de datos, llamando al procedimiento almacenado correspondiente. Devuelve el resultado del procedimiento.
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

    //registra la transacción en la bitácora en formato JSON
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