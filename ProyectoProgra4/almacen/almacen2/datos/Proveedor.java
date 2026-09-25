package almacen.almacen2.datos;

/**
 * Representa un Proveedor dentro del sistema de Almacen (historia ALMACEN2).
 *
 * no valida nada,
 * no habla con la base de datos
 *  Su unico trabajo es guardar, de forma ordenada y con tipos de
 * dato correctos, la informacion de un proveedor una vez que ya fue
 * extraida de la trama.
 *
 *
 */
public class Proveedor {

    // Cedula juridica del proveedor. Es la llave primaria en la base de
    // datos 
    private final String cedulaJuridica;

    // nombre de la empresa proveedora.
    private String nombre;

    // Nombre de la persona de contacto dentro de esa empresa.
    private String nombreContacto;

    // Telefono de contacto. Se guarda como String (no como numero) porque
    // puede empezar en cero y porque nunca se hacen operaciones
    private String telefono;

    // Correo electronico de contacto.
    private String correo;

    // Activo o Inactivo. Se usa el enum EstadoProveedor en vez de un
    // String o un char para que sea imposible representar un estado
    // que no exista.
    private EstadoProveedor estado;

    /**
     * Construye un Proveedor completo.
     *
     * Se exige la cedula juridica en el constructor  porque es el identificador del proveedor: no
     * tiene sentido de negocio que un Proveedor cambie de cedula despues
     * de creado. El resto de los campos si tienen su "setter" porque es lo que hace una Modificacion
     */
    public Proveedor(String cedulaJuridica, String nombre, String nombreContacto,
                      String telefono, String correo, EstadoProveedor estado) {
        this.cedulaJuridica = cedulaJuridica;
        this.nombre = nombre;
        this.nombreContacto = nombreContacto;
        this.telefono = telefono;
        this.correo = correo;
        this.estado = estado;
    }

    public String getCedulaJuridica() {
        return cedulaJuridica;
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public String getNombreContacto() {
        return nombreContacto;
    }

    public void setNombreContacto(String nombreContacto) {
        this.nombreContacto = nombreContacto;
    }

    public String getTelefono() {
        return telefono;
    }

    public void setTelefono(String telefono) {
        this.telefono = telefono;
    }

    public String getCorreo() {
        return correo;
    }

    public void setCorreo(String correo) {
        this.correo = correo;
    }

    public EstadoProveedor getEstado() {
        return estado;
    }

    public void setEstado(EstadoProveedor estado) {
        this.estado = estado;
    }

    /**
     * solo para depurar en la consola. No se usa para nada mas. 
     */
    @Override
    public String toString() {
        return "Proveedor{" +
                "cedulaJuridica='" + cedulaJuridica + '\'' +
                ", nombre='" + nombre + '\'' +
                ", nombreContacto='" + nombreContacto + '\'' +
                ", telefono='" + telefono + '\'' +
                ", correo='" + correo + '\'' +
                ", estado=" + estado +
                '}';
    }
}