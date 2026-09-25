package almacen.almacen2.datos;

/**
 * Representa el campo "Estado" de la trama de ALMACEN2.
 *
    Define este campo como 1 caracter con dos valores posibles:
 *   1 = Activo
 *   2 = Inactivo
 *
 * En vez de manejar ese caracter como un String se encierra 
 * en un enum, obliga a usar solo uno de
 * estos dos valores en el resto del programa.
 */
public enum EstadoProveedor {

    ACTIVO("1"),//constante para indicar que se trata de un proveedor activo
    INACTIVO("2");//constante para indicar que se trata de un proveedor inactivo

    // Codigo de un solo caracter tal como viaja dentro de la trama de texto plano.
    private final String codigoTrama;

    // Constructor privado ya que los enums en Java no se instancian desde afuera
    // solo existen las dos constantes declaradas arriba
    EstadoProveedor(String codigoTrama) {
        this.codigoTrama = codigoTrama;
    }

    /**
     * Devuelve el caracter exacto que debe escribirse en la posicion 270
        * de la trama de texto plano para representar este estado.
     */
    public String getCodigoTrama() {
        return codigoTrama;
    }

    /**
     * Convierte el caracter recibido en la trama al enum
     * correspondiente.
     *
     * Se usa cuando el socket recibe la trama del Almacen y necesita
     * transformar el texto plano en un objeto Proveedor: en vez de guardar
     * el caracter "1" o "2" tal cual, se guarda ya validado como
     * EstadoProveedor.ACTIVO o EstadoProveedor.INACTIVO.
     *
     * @param codigoTrama el caracter recibido, tal cual vino en la trama
     * @return el enum correspondiente
     * @throws IllegalArgumentException si el caracter no es "1" ni "2".
     *         Esta excepcion NO se le muestra al usuario final: la capa de
     *         servicio (que se hara en la siguiente parte) debe atraparla
     *         y responder "DATO INVALIDO"
     */
    public static EstadoProveedor fromCodigoTrama(String codigoTrama) {
        for (EstadoProveedor estado : values()) {
            if (estado.codigoTrama.equals(codigoTrama)) {
                return estado;
            }
        }
        throw new IllegalArgumentException(
            "Estado invalido: '" + codigoTrama + "'. Se esperaba '1' (Activo) o '2' (Inactivo)."
        );
    }
}