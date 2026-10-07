#verificador 1 : mantenimiento de clientes 
#recibe la trama ya convertida a dicionario y devuelve un diccionario 

import mysql.connector
import DB
from Logica import Validaciones as val 

CAMPOS = ("identificacion", "pais", "nombre", "primer_apellido", 
          "segundo_apellido", "correo", "telefono", "direccion", "transaccion")
TRANSACCIONES = ("agregar", "modificar", "borrar")

DATOS_INVALIDADOS = {"status":"1"}
CLIENTE_YA_EXISTE = {"status":"2"}
CLIENTE_NO_EXISTE = {"status":"3"}
ERROR_NO_CONTROLADO = {"status":"4"}
EXITO = {"status":"OK"}
CON_HISTORIAL = {"status":"Error: cliente con historial de compras"}

#codigos de error en mysql
ERR_LLAVE_DUPLICADA = 1062 #INSERT LLAVE PRIMARIA QUE YA EXISTE
ERR_LLAVE_FORANEA = 1451 #DELETE UN CLIENTE QUE TIENE COMPRAS 

def _limpiar(datos):
    """Revisa que vengas todos los campos como texto no vacio y devuelve una copia"""
    if not isinstance(datos, dict):
        return None
    cliente = {}
    for campo in CAMPOS:
        valor = datos.get(campo)
        if not val.es_texto(valor):
            return None
        cliente[campo] = valor.strip()
    cliente["pais"] = cliente["pais"].upper()
    cliente["transaccion"] = cliente["transaccion"].lower()
    
    telefono = val.normalizar_telefono(cliente["telefono"])
    if telefono is None:
        return None
    cliente["telefono"] = telefono 
    return cliente

def _datos_validos(c): 
    """Devuelve true solo si todos los datos cumplen las validaciones"""
    return (c["transaccion"] in TRANSACCIONES 
            and val.pais_valido(c["pais"])
            and val.identificacion_valida(c["identificacion"], c["pais"])
            and val.nombre_valido(c["nombre"])
            and val.nombre_valido(c["primer_apellido"])
            and val.nombre_valido(c["segundo_apellido"])
            and val.correo_valido(c["correo"])
            and val.direccion_valida(c["direccion"]))
            
def _argumentos(c):
    """Datos en orden exato a los parametros del procedimiento"""
    return(c["identificacion"], c["pais"], c["nombre"],
           c["primer_apellido"], c["segundo_apellido"], c["correo"],
           c["telefono"], c["direccion"])
    
def _limpiar_borrado(datos):
    """Para borrar solo hacen falta identificacion y pais"""
    identificacion = datos.get("identificacion")
    pais = datos.get("pais")
    if not (val.es_texto(identificacion) and val.es_texto(pais)):
        return None
    cliente = {"identificacion": identificacion.strip(),
               "pais": pais.strip().upper(),
               "transaccion": "borrar"}
    if not (val.pais_valido(cliente["pais"])
            and val.identificacion_valida(cliente["identificacion"], cliente["pais"])):
        return None
    return cliente

def procesar(datos):
    """Punto de entrada: recibe el diccionario de la trama y devuelve el diccionario de respuesta"""
    try: 
        es_borrado = (isinstance(datos, dict)
                      and str(datos.get("transaccion", "")).strip().lower() == "borrar" )
        if es_borrado: 
            cliente = _limpiar_borrado(datos)
            if cliente is None: 
                return DATOS_INVALIDADOS
        else: 
            cliente = _limpiar(datos)
            if cliente is None or not _datos_validos(cliente):
                return DATOS_INVALIDADOS
        
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
                 
    
    