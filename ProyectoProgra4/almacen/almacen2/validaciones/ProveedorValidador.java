package almacen.almacen2.validaciones;


import almacen.almacen2.datos.EstadoProveedor;
import almacen.almacen2.datos.Proveedor;

import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

/**
 * Valida las reglas de negocio de un Proveedor,
 * cubre unicamente las validaciones de FORMATO:
 *   - cedula juridica: solo digitos, tamano correcto
 *   - nombre / nombre de contacto: no vacios, no numericos
 *   - telefono: valido para Costa Rica
 *   - correo: formato de correo valido
 *   - estado:
 */
public class ProveedorValidador {

    // Cedula juridica
    // 10 digitos, sin letras ni espacios 
    private static final Pattern PATRON_CEDULA_JURIDICA = Pattern.compile("^\\d{10}$");

    // Un texto que es de solo numeros si, una vez quitados los espacios,
    // esta formado solo por digitos del 0 al 9. Se usa para rechazar nombres
    // con numeros tanto en el nombre de la empresa como en el del
    // contacto.
    private static final Pattern PATRON_SOLO_NUMEROS = Pattern.compile("^\\d+$");

    // Telefono de exactamente 8 digitos, y el primer digito debe ser 2,3,4,5,6,7 u 8.
    private static final Pattern PATRON_TELEFONO_CR = Pattern.compile("^[24-8]\\d{7}$");

    // Formato de correo generico 
    private static final Pattern PATRON_CORREO = Pattern.compile(
        "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$"
    );

    /**
     * Revisa todas las reglas de formato sobre un Proveedor y devuelve la
     * lista de errores encontrados. Lista vacia significa que el
     * Proveedor pasa todas las validaciones de formato.
     */
    public List<String> validar(Proveedor proveedor) {
        List<String> errores = new ArrayList<>();

        validarCedulaJuridica(proveedor.getCedulaJuridica(), errores);
        validarTextoNoNumerico(proveedor.getNombre(), "El nombre del proveedor", errores);
        validarTextoNoNumerico(proveedor.getNombreContacto(), "El nombre de contacto", errores);
        validarTelefono(proveedor.getTelefono(), errores);
        validarCorreo(proveedor.getCorreo(), errores);

        return errores;
    }

    /**
     * Atajo para cuando el servicio solo necesita saber si-o-no, sin el
     * detalle de que fallo.
     */
    public boolean esValido(Proveedor proveedor) {
        return validar(proveedor).isEmpty();
    }

    //validaciones individuales de cada campo, para poder dar mensajes de error
    private void validarCedulaJuridica(String cedulaJuridica, List<String> errores) {
        if (cedulaJuridica == null || !PATRON_CEDULA_JURIDICA.matcher(cedulaJuridica).matches()) {
            errores.add("La cedula juridica debe tener exactamente 10 digitos numericos.");
        }
    }

    /**
     * Se usa tanto para "nombre" como para "nombre de contacto", no numericos y vacios
     */
    private void validarTextoNoNumerico(String texto, String nombreCampo, List<String> errores) {
        if (texto == null || texto.isBlank()) {
            errores.add(nombreCampo + " no puede estar vacio ni ser solo espacios en blanco.");
            return;
        }
        if (PATRON_SOLO_NUMEROS.matcher(texto).matches()) {
            errores.add(nombreCampo + " no puede ser puramente numerico.");
        }
    }

    // Valida que el telefono sea un numero valido de Costa Rica (8 digitos, primer digito 2-8)
    private void validarTelefono(String telefono, List<String> errores) {
        if (telefono == null || !PATRON_TELEFONO_CR.matcher(telefono).matches()) {
            errores.add("El telefono debe ser un numero valido de Costa Rica (8 digitos).");
        }
    }

    // Valida que el correo tenga un formato de correo valido
    private void validarCorreo(String correo, List<String> errores) {
        if (correo == null || !PATRON_CORREO.matcher(correo).matches()) {
            errores.add("El correo electronico no tiene un formato valido.");
        }
    }
}