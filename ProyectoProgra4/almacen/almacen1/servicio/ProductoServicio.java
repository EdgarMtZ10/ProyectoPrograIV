package almacen.almacen1.servicio;

import almacen.almacen1.datos.Producto;
import almacen.almacen1.datos.ProductoParser;
import almacen.almacen1.validaciones.ProductoValidador;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Types;
import java.util.List;

import almacen.almacen5.Bitacora;

/*
  Servicio de Productos del almacen 1
 
  Hace el flujo completo de un ingreso o modificacion de producto:
  1. Extrae los datos de la trama fija.
  2. Valida las reglas de negocio.
  3. Ejecuta la transaccion en la base de datos (segun el tipo).
  4. Devuelve el texto exacto de respuesta.
 */
public class ProductoServicio {

    private static final String TIPO_INGRESO = "1";
    private static final String TIPO_MODIFICACION = "2";

    private final ProductoParser parser;
    private final ProductoValidador validador;
    private final Connection conexion;

    // Constructor que recibe la conexion a la base de datos
    public ProductoServicio(Connection conexion) {
        this.parser = new ProductoParser();
        this.validador = new ProductoValidador();
        this.conexion = conexion;
    }

    /**
     * Procesa la trama completa y devuelve el resultado de la operacion.
     */
    public String procesar(String tramaCruda)
     {
        ProductoParser.TramaProducto tramaParseada;

        // Parseo de la trama (si la longitud o formato base falla, es DATO INVALIDO)
        try {
            tramaParseada = parser.parsear(tramaCruda);
        } catch (IllegalArgumentException e) {
            return "DATO INVALIDO";
        }

        Producto producto = tramaParseada.getProducto();

        // Validacion de reglas de negocio (sin tocar BD)
        List<String> errores = validador.validar(producto);
        if (!errores.isEmpty()) {
            return "DATO INVALIDO";
        }

                // Ejecucion en base de datos segun el tipo de transaccion
        String resultado;
        try {
            if (tramaParseada.getTipoTransaccion().equals(TIPO_INGRESO)) {
                resultado = ingresarProducto(producto);
            } else if (tramaParseada.getTipoTransaccion().equals(TIPO_MODIFICACION)) {
                resultado = modificarProducto(producto);
            } else {
                resultado = "DATO INVALIDO";
            }
        } catch (SQLException e) {
            resultado = "ERROR";
        }

        registrarBitacora(tramaParseada.getTipoTransaccion(), producto);
        return resultado;
    }


    /**
     * Llama a sp_ingresar_producto para insertar un producto nuevo.
     * El trigger en SQL Server se encarga de crear el inventario en cero.
     */
    private String ingresarProducto(Producto producto) throws SQLException {
        String sql = "{call sp_ingresar_producto(?, ?, ?, ?)}";

        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            llamada.setString(1, producto.getNoProducto());
            llamada.setString(2, producto.getNombre());
            llamada.setBigDecimal(3, producto.getPrecio());

            llamada.registerOutParameter(4, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(4);
        }
    }

    /**
     * Llama a sp_modificar_producto para actualizar un producto existente.
     */
    private String modificarProducto(Producto producto) throws SQLException {
        String sql = "{call sp_modificar_producto(?, ?, ?, ?)}";// Llamada al procedimiento almacenado para modificar un producto

        //se usa try-with-resources para asegurar que el CallableStatement se cierre automáticamente
        try (CallableStatement llamada = conexion.prepareCall(sql)) {
            llamada.setString(1, producto.getNoProducto());
            llamada.setString(2, producto.getNombre());
            llamada.setBigDecimal(3, producto.getPrecio());

            llamada.registerOutParameter(4, Types.VARCHAR);
            llamada.execute();

            return llamada.getString(4);
        }
    }

    private void registrarBitacora(String tipoTransaccion, Producto producto) {// Formato JSON para la bitacora
        String json = String.format(
            // Se escapan las comillas dobles de los nombres y se registra el estado como su codigo de trama.
            "{\"Transaccion\": \"%s\", \"No Producto\": \"%s\", \"Nombre\": \"%s\", \"Precio\": \"%s\"}",
            tipoTransaccion,
            producto.getNoProducto(),
            producto.getNombre().replace("\"", "\\\""),
            producto.getPrecio().toPlainString()
        );
        Bitacora.getInstancia().registrar(json);
    }
}