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
        try {
            // Apagamos el auto-commit para asegurar que si falla un producto, 
            // no se guarde la cabecera ni los productos anteriores (todo o nada).
            conexion.setAutoCommit(false);

            // Registrar la cabecera de la compra
            String resultadoCabecera = registrarCabecera(compra);
            if (!resultadoCabecera.equals("EXITOSO")) {
                conexion.rollback(); // Deshacer en caso de PROVEEDOR INVÁLIDO o DATO INVÁLIDO
                return resultadoCabecera; 
            }

            // Registrar cada producto en la tabla de detalles
            for (CompraDetalle detalle : compra.getDetalles()) {
                String resultadoDetalle = agregarDetalle(compra.getNumeroIngreso(), detalle);
                
                if (!resultadoDetalle.equals("EXITOSO")) {
                    conexion.rollback(); // Si un producto es PRODUCTO INVÁLIDO, se cancela toda la compra
                    return resultadoDetalle;
                }
            }

            // Si llegamos aquí, la cabecera y todos los productos se insertaron bien
            conexion.commit();
            return "EXITOSO";

        } catch (SQLException e) {
            // Si ocurre un error a nivel de SQL 
            try {
                conexion.rollback();
            } catch (SQLException rollbackEx) {
                // Se ignora el error para devolver el error principal
            }
            return "ERROR";
        } finally {
            // Siempre restauramos el comportamiento por defecto de la conexión
            try {
                conexion.setAutoCommit(true);
            } catch (SQLException e) {
                // Ignorado
            }
        }
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
}