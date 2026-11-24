#verificador 1 : mantenimiento de clientes 
#recibe la trama ya convertida a dicionario y devuelve un diccionario 

import mysql.connector
import DB
from Logica import Validaciones as val 

CAMPOS = ("identificacion", "pais", "nombre", "primer_apellido", 
          "segundo_apellido", "correo", "telefono", "direccion", "transaccion")
TRANSACCIONES = ("agregar", "modificar", "borrar")

#se agrega errores con el campo exacto
def _error(status, errores):
    return {"status": status,
            "errores": [{"campo": campo, "detalle": detalle} for campo, detalle in errores]}

CLIENTE_YA_EXISTE = _error("2", [("identificacion", "Ya existe un cliente con esa identificación.")])
CLIENTE_NO_EXISTE = _error("3", [("identificacion", "No existe ningún cliente con esa identificación.")])
ERROR_NO_CONTROLADO = _error("4", [("verificador", "Ocurrió un error no controlado al procesar la solicitud.")])
EXITO = {"status":"OK"}
CON_HISTORIAL = _error("Error: cliente con historial de compras",
                       [("identificacion", "No se puede borrar: el cliente tiene compras registradas.")])

#codigos de error en mysql
ERR_LLAVE_DUPLICADA = 1062 #INSERT LLAVE PRIMARIA QUE YA EXISTE
ERR_LLAVE_FORANEA = 1451 #DELETE UN CLIENTE QUE TIENE COMPRAS 

def _validar(datos):
    """Revisa todos los campos y devuelve (cliente, errores) acumulando cada campo que falla"""
    if not isinstance(datos, dict):
        return None, [("trama", "La trama del cliente no tiene el formato correcto.")]
    errores = []
    cliente = {}
    for campo in CAMPOS:
        valor = datos.get(campo)
        if not val.es_texto(valor):
            errores.append((campo, "Es obligatorio."))
            cliente[campo] = ""
        else:
            cliente[campo] = valor.strip()
    cliente["pais"] = cliente["pais"].upper()
    cliente["transaccion"] = cliente["transaccion"].lower()

    if cliente["transaccion"] and cliente["transaccion"] not in TRANSACCIONES:
        errores.append(("transaccion", "Debe ser agregar, modificar o borrar."))
    if cliente["pais"] and not val.pais_valido(cliente["pais"]):
        errores.append(("pais", "Debe ser un código de país de 2 letras (ISO), por ejemplo CR."))
    if cliente["identificacion"] and cliente["pais"] and val.pais_valido(cliente["pais"]) \
            and not val.identificacion_valida(cliente["identificacion"], cliente["pais"]):
        errores.append(("identificacion", "Para CR: cédula física de 9 dígitos o jurídica de 10 que inicia en 3. "
                                          "Otros países: 6 a 20 letras o números."))
    for campo in ("nombre", "primer_apellido", "segundo_apellido"):
        if cliente[campo] and not val.nombre_valido(cliente[campo]):
            errores.append((campo, f"Solo letras, máximo {val.LARGO_NOMBRE} caracteres."))
    if cliente["correo"] and not val.correo_valido(cliente["correo"]):
        errores.append(("correo", f"Debe ser un correo válido de máximo {val.LARGO_CORREO} caracteres."))
    if cliente["telefono"]:
        telefono = val.normalizar_telefono(cliente["telefono"])
        if telefono is None:
            errores.append(("telefono", "Debe ser un teléfono de Costa Rica de 8 dígitos."))
        else:
            cliente["telefono"] = telefono
    return cliente, errores
            
def _argumentos(c):
    """Datos en orden exato a los parametros del procedimiento"""
    return(c["identificacion"], c["pais"], c["nombre"],
           c["primer_apellido"], c["segundo_apellido"], c["correo"],
           c["telefono"], c["direccion"])
    
def _validar_borrado(datos):
    """Para borrar solo hacen falta identificacion y pais"""
    errores = []
    identificacion = datos.get("identificacion")
    pais = datos.get("pais")
    if not val.es_texto(identificacion):
        errores.append(("identificacion", "Es obligatorio."))
    if not val.es_texto(pais):
        errores.append(("pais", "Es obligatorio."))
    if errores:
        return None, errores
    cliente = {"identificacion": identificacion.strip(),
               "pais": pais.strip().upper(),
               "transaccion": "borrar"}
    if not val.pais_valido(cliente["pais"]):
        errores.append(("pais", "Debe ser un código de país de 2 letras (ISO), por ejemplo CR."))
    elif not val.identificacion_valida(cliente["identificacion"], cliente["pais"]):
        errores.append(("identificacion", "La identificación no es válida para el país indicado."))
    return cliente, errores

def procesar(datos):
    """Punto de entrada: recibe el diccionario de la trama y devuelve el diccionario de respuesta"""
    try: 
        es_borrado = (isinstance(datos, dict)
                      and str(datos.get("transaccion", "")).strip().lower() == "borrar" )
        if es_borrado: 
            cliente, errores = _validar_borrado(datos)
        else: 
            cliente, errores = _validar(datos)
        if errores:
            return _error("1", errores)
        
        transaccion = cliente["transaccion"]
        existe = DB.existe_cliente(cliente["identificacion"])
        
        if transaccion == "agregar":
            if existe: 
                return CLIENTE_YA_EXISTE
            DB.registrar_cliente(*_argumentos(cliente))
            
        elif transaccion == "modificar":
            if not existe:
                return CLIENTE_NO_EXISTE
            DB.modificar_cliente(*_argumentos(cliente))
        else: 
            if not existe:
                return CLIENTE_NO_EXISTE
            if DB.cliente_tiene_compras(cliente["identificacion"]):
                return CON_HISTORIAL
            DB.borrar_cliente(cliente["identificacion"])
        return EXITO
    except mysql.connector.Error as error: 
        #se usa mysql como ultima linea validacion/defensa
        if error.errno == ERR_LLAVE_DUPLICADA:
            return CLIENTE_YA_EXISTE
        if error.errno == ERR_LLAVE_FORANEA:
            return CON_HISTORIAL
        print(f"[VERIFICADOR1] Error de MySQL: {error}")
        return ERROR_NO_CONTROLADO
    
    except Exception as error: 
        print(f"[VERIFICADOR1] Error no controlado: {error}")
        return ERROR_NO_CONTROLADO
                 
    
    