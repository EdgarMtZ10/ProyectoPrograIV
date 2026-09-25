package almacen.almacen2.datos;

/**
 * Representa el campo "Tipo de transaccion" de la trama de almacen2.
 *
 * Define este campo como 1 caracter con dos valores posibles:
 */
public enum TipoTransaccionProveedor {

    INGRESO("3"), //constante para indicar que se trata de un ingreso de proveedor
    MODIFICACION("4");//constante para indicar que se trata de una modificacion de proveedor

    private final String codigoTrama;//almacena el codigo de un solo caracter tal como viaja dentro de la trama

    TipoTransaccionProveedor(String codigoTrama) {
        this.codigoTrama = codigoTrama;//constructor privado: los enums en Java no se instancian desde afuera, solo existen las dos constantes declaradas arriba (INGRESO y MODIFICACION).
    }

    //metodo de acceso para obtener el string que representa el codigo asociado a laconstante del enum
    public String getCodigoTrama() {
        return codigoTrama;
    }

    /**
     * Convierte el caracter recibido en la trama (posicion 1) al enum
     * correspondiente. Igual que en EstadoProveedor: si el caracter no es
     * valido, se lanza una excepcion que la capa de servicio debe atrapar
     * y traducir a la respuesta "DATO INVALIDO".
     */
    public static TipoTransaccionProveedor fromCodigoTrama(String codigoTrama) {
        for (TipoTransaccionProveedor tipo : values()) {
            if (tipo.codigoTrama.equals(codigoTrama)) {
                return tipo;
            }
        }
        throw new IllegalArgumentException(
            "Tipo de transaccion invalido: '" + codigoTrama + "'. Se esperaba '3' (Ingreso) o '4' (Modificacion)."
        );
    }
}