#verificador 2 , registro de compras 

import threading
import traceback

import DB
from Logica import Almacen
from Logica import Validaciones as val
from Logica import Verificador3 as bitacora 

CAMPOS = ("numero_compra","id_cliente","fecha_compra", "total_compra", "num_tarjeta", "fecha_vencimiento", 
          "codigo_verificacion")
CAMPOS_CIFRADOS = ("num_tarjeta", "fecha_vencimiento", "codigo_verificacion")
#respuestas a acciones
STATUS_VALIDACION = "1"
STATUS_PROCESO = "2"
EXITO = {"status":"OK"}
#limites de trama 
MAX_DIGITOS_COMPRA = 10
MAX_DIGITOS_PRODUCTO = 10
MAX_DIGITOS_CANTIDAD = 7
MAX_LARGO_CLIENTE = 20          # ID_CLIENTE VARCHAR(20)
MAX_TOTAL = 99999999.99         # TOTAL_COMPRA DECIMAL(10,2)

#respuestas del almacen
ERRORES_ALMACEN = {
    "DATO INVALIDO": ("productos", "El almacén rechazó el formato de los productos."),
    "PRODUCTO INVALIDO": ("productos", "Uno de los productos no existe en el almacén."),
    "CANTIDAD INSUF": ("productos", "No hay inventario suficiente para uno de los productos."),
    "ERROR": ("numero_compra", "El almacén no pudo registrar la salida (el número de compra podría existir ya en el almacén)."),
}

#numeros de compra en hilos 
_en_proceso = set()
_candado = threading.Lock()


def _error(status, errores):
    """Respuesta de error con la lista de campos que fallaron"""
    return {"status": status,
            "errores": [{"campo": campo, "detalle": detalle} for campo, detalle in errores]}

def _reservar(numero):
    """Marca el numero en proceso y regresa false si otro hilo lo tiene ya"""
    with _candado: 
        if numero in _en_proceso:
            return False
        _en_proceso.add(numero)
        return True
    
def _liberar(numero):
    with _candado:
        _en_proceso.discard(numero)
        
def _validar_productos(lista, errores):
    """Revisa cada linea y devuelve la lista de tuplas (codigo, cantidad); anota todos los errores"""
    if not isinstance(lista, list) or not lista:
        errores.append(("productos", "Debe incluir al menos un producto."))
        return []
    productos = []
    vistos = set()
    for numero, item in enumerate(lista, start=1):
        prefijo = f"productos[{numero}]"
        if not isinstance(item, dict):
            errores.append((prefijo, "La línea no tiene el formato correcto."))
            continue
        codigo = item.get("codigo_producto")
        cantidad = item.get("cantidad")
        codigo = codigo.strip() if val.es_texto(codigo) else ""
        cantidad = cantidad.strip() if val.es_texto(cantidad) else ""

        linea_ok = True
        if not val.entero_positivo(codigo, MAX_DIGITOS_PRODUCTO):
            errores.append((prefijo + ".codigo_producto",
                            f"Debe ser numérico, mayor que cero y de máximo {MAX_DIGITOS_PRODUCTO} dígitos."))
            linea_ok = False
        elif codigo.zfill(MAX_DIGITOS_PRODUCTO) in vistos:
            #el almacen revisa el inventario linea por linea: un codigo repetido podria vender de mas
            errores.append((prefijo + ".codigo_producto", f"El producto {codigo} está repetido en la compra."))
            linea_ok = False
        if not val.entero_positivo(cantidad, MAX_DIGITOS_CANTIDAD):
            errores.append((prefijo + ".cantidad",
                            f"Debe ser numérica, mayor que cero y de máximo {MAX_DIGITOS_CANTIDAD} dígitos."))
            linea_ok = False
        if linea_ok:
            vistos.add(codigo.zfill(MAX_DIGITOS_PRODUCTO))
            productos.append((codigo, cantidad))
    return productos

def _validar(datos):
    """Revisa todos los campos y devuelve (compra, errores). Se acumulan todos los errores, no solo el primero"""
    errores = []
    if not isinstance(datos, dict):
        return None, [("trama", "La trama de la compra no tiene el formato correcto.")]

    compra = {}
    for campo in CAMPOS:
        valor = datos.get(campo)
        if not val.es_texto(valor):
            errores.append((campo, "Es obligatorio."))
            compra[campo] = ""
        else:
            compra[campo] = valor.strip()

    numero = compra["numero_compra"]
    if numero:
        if not val.entero_positivo(numero, MAX_DIGITOS_COMPRA):
            errores.append(("numero_compra", f"Debe ser numérico, mayor que cero y de máximo {MAX_DIGITOS_COMPRA} dígitos."))
        else:
            compra["numero_compra"] = str(int(numero))

    if compra["id_cliente"] and len(compra["id_cliente"]) > MAX_LARGO_CLIENTE:
        errores.append(("id_cliente", f"Máximo {MAX_LARGO_CLIENTE} caracteres."))

    compra["fecha"] = None
    if compra["fecha_compra"]:
        compra["fecha"] = val.leer_fecha_de_hoy(compra["fecha_compra"])
        if compra["fecha"] is None:
            errores.append(("fecha_compra", "Debe tener formato yyyyMMdd y ser la fecha de hoy."))

    compra["total"] = None
    if compra["total_compra"]:
        compra["total"] = val.leer_monto(compra["total_compra"])
        if compra["total"] is None:
            errores.append(("total_compra", "Debe ser un monto positivo con punto y máximo 2 decimales."))
        elif compra["total"] > MAX_TOTAL:
            errores.append(("total_compra", "Supera el monto máximo permitido."))

    for campo in CAMPOS_CIFRADOS:
        if compra[campo] and not val.parece_cifrado(compra[campo]):
            errores.append((campo, "No viene cifrado correctamente."))

    compra["productos"] = _validar_productos(datos.get("productos"), errores)
    return compra, errores
    
def _registrar(c):
    """validaciones con la base, consulta al almacen y guardado"""
    errores = []
    if DB.existe_compra(c["numero_compra"]):
        errores.append(("numero_compra", "Ya existe una compra con ese número."))
    if not DB.existe_cliente(c["id_cliente"]):
        errores.append(("id_cliente", "El cliente no está registrado en el verificador."))
    if errores:
        return _error(STATUS_VALIDACION, errores)
    
    trama = Almacen.armar_trama_salida(c["numero_compra"], c["fecha_compra"], c["productos"])
    bitacora.registrar_salida(trama, destino="Almacen") #llamada interna al almacen
    try:
        respuesta = Almacen.enviar(trama)
    except OSError:
        bitacora.registrar("SIN RESPUESTA", origen="Almacen")
        return _error(STATUS_PROCESO, [("almacen", "No se pudo conectar con el almacén. Verifique que esté encendido.")])
    bitacora.registrar(respuesta, origen="Almacen") #respuesta del almacen
    
    if respuesta != Almacen.EXITOSO:
        print(f"[VERIFICADOR2] EL almacen rechazo la compra {c['numero_compra']}: {respuesta}")
        campo, detalle = ERRORES_ALMACEN.get(respuesta, ("almacen", f"El almacén rechazó la compra ({respuesta})."))
        return _error(STATUS_PROCESO, [(campo, detalle)])
    
    productos = [(codigo, int(cantidad)) for codigo, cantidad in c["productos"]]
    DB.registrar_compra(c["numero_compra"], c["id_cliente"], c["fecha"], c["total"], 
                       c["num_tarjeta"], c["fecha_vencimiento"], c["codigo_verificacion"], productos)
    return EXITO

def procesar(datos):
    """recibe el diccionario de la trama y devuelve el diccionario de respuesta"""
    try: 
        compra, errores = _validar(datos)
        if errores:
            return _error(STATUS_VALIDACION, errores)
        numero = compra["numero_compra"]
        if not _reservar(numero):
            return _error(STATUS_VALIDACION, [("numero_compra", "Esa compra ya se está procesando.")])
        try: 
            return _registrar(compra)
        finally: 
            _liberar(numero)
            
    except Exception as error:
        #nunca se imprime la trama: podria llevar datos de la tarjeta
        print(f"[VERIFICADOR2] Error al procesar la compra: {type(error).__name__}: {error}")
        traceback.print_exc()
        return _error(STATUS_PROCESO, [("verificador", "Error no controlado al registrar la compra.")])