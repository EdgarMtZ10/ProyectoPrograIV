package almacen.almacen2.datos;


/**
 * Parser de la trama de texto plano de ALMACEN2 
 *
 * Su unico trabajo es tomar el String de 270 caracteres que llega
 * por el socket y convertirlo en un objeto  o en un Proveedor, segun corresponda. No valida
 * reglas de negocio (eso lo hace el validador en la siguiente parte), y
 * no habla con la base de datos ni con el socket directamente.
 *
 * Se separa el tipo de transaccion del resto de los datos porque ese
 * campo es el que decide, mas adelante en el servicio
 */
public class ProveedorParser {

    // Posiciones segun el contrato de tramas 
    // Se guardan como constantes con nombre ,
    // solo haya que tocar estos numeros en un solo lugar,
    private static final int INICIO_TIPO_TRANSACCION = 1;
    private static final int TAMANO_TIPO_TRANSACCION = 1;

    private static final int INICIO_CEDULA_JURIDICA = 2;
    private static final int TAMANO_CEDULA_JURIDICA = 10;

    private static final int INICIO_NOMBRE = 12;
    private static final int TAMANO_NOMBRE = 100;

    private static final int INICIO_NOMBRE_CONTACTO = 112;
    private static final int TAMANO_NOMBRE_CONTACTO = 75;

    private static final int INICIO_TELEFONO = 187;
    private static final int TAMANO_TELEFONO = 8;

    private static final int INICIO_CORREO = 195;
    private static final int TAMANO_CORREO = 75;

    private static final int INICIO_ESTADO = 270;
    private static final int TAMANO_ESTADO = 1;

    // Largo de la trama. Sirve para detectar de una vez
    // si llego una trama incompleta
    public static final int LARGO_TRAMA = 270;

    /**
     * Convierte la trama en un TramaProveedor.
     * IllegalArgumentException si la trama no tiene el largo
     *         esperado. Igual que con los enums, esta excepcion la debe
     *         atrapar la capa de servicio y responder "DATO INVALIDO".
     */
    public TramaProveedor parsear(String trama) {
        if (trama == null || trama.length() != LARGO_TRAMA) {
            throw new IllegalArgumentException(
                "La trama de proveedor debe tener exactamente " + LARGO_TRAMA +
                " caracteres. Se recibieron: " + (trama == null ? 0 : trama.length())
            );
        }

        String tipoTransaccionTexto = extraerCampo(trama, INICIO_TIPO_TRANSACCION, TAMANO_TIPO_TRANSACCION);
        String cedulaJuridica       = extraerCampo(trama, INICIO_CEDULA_JURIDICA, TAMANO_CEDULA_JURIDICA);
        String nombre               = extraerCampo(trama, INICIO_NOMBRE, TAMANO_NOMBRE);
        String nombreContacto       = extraerCampo(trama, INICIO_NOMBRE_CONTACTO, TAMANO_NOMBRE_CONTACTO);
        String telefono             = extraerCampo(trama, INICIO_TELEFONO, TAMANO_TELEFONO);
        String correo               = extraerCampo(trama, INICIO_CORREO, TAMANO_CORREO);
        String estadoTexto          = extraerCampo(trama, INICIO_ESTADO, TAMANO_ESTADO);

        // Aqui se convierten los strings sueltos en los tipos correctos.
        // Si alguno de estos codigos no es valido, fromCodigoTrama lanza
        // la excepcion y sube asi hasta el servicio.
        TipoTransaccionProveedor tipoTransaccion = TipoTransaccionProveedor.fromCodigoTrama(tipoTransaccionTexto);
        EstadoProveedor estado = EstadoProveedor.fromCodigoTrama(estadoTexto);

        Proveedor proveedor = new Proveedor(
            cedulaJuridica, nombre, nombreContacto, telefono, correo, estado
        );

        return new TramaProveedor(tipoTransaccion, proveedor);
    }

    /**
     * Extrae un campo de ancho fijo de la trama y le quita los espacios
     * sobrantes al inicio/final (padding). El PDF dice que el nombre, el
     * contacto, etc. rellenan su espacio fijo con espacios en blanco
     * cuando el texto real es mas corto que el tamano del campo (ej. un
     * nombre de 40 letras dentro de un espacio de 100) - sin este trim,
     * el nombre quedaria guardado con 60 espacios en blanco pegados al
     * final, lo cual rompe comparaciones como "el nombre no debe existir
     * previamente" en el validador.
     *
     * @param trama      la trama completa
     * @param inicio1based posicion inicial tal como la escribe el PDF (empieza en 1)
     * @param tamano     cantidad de caracteres que ocupa el campo
     */
    private String extraerCampo(String trama, int inicio1based, int tamano) {
        // Conversion clave
        // cuenta desde 0. Por eso se resta 1 al inicio.
        int inicio0based = inicio1based - 1;
        int fin0based = inicio0based + tamano; 

        return trama.substring(inicio0based, fin0based).trim();
    }

    /**
     * clase contenedora
     * necesita AMBOS datos para decidir si hacer un Ingreso o una
     * Modificacion. No se mete el tipo de transaccion dentro de la clase
     * Proveedor porque el tipo de transaccion no es un dato del
     * proveedor en si, es un dato de la operacion que se esta pidiendo.
     */
    public static class TramaProveedor {
        private final TipoTransaccionProveedor tipoTransaccion;
        private final Proveedor proveedor;

        public TramaProveedor(TipoTransaccionProveedor tipoTransaccion, Proveedor proveedor) {
            this.tipoTransaccion = tipoTransaccion;
            this.proveedor = proveedor;
        }

        public TipoTransaccionProveedor getTipoTransaccion() {
            return tipoTransaccion;
        }

        public Proveedor getProveedor() {
            return proveedor;
        }
    }
}
