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
ERROR_VALIDACION = {"status":"1"}
ERROR_PROCESO = {"status":"2"}
EXITO = {"status":"OK"}
#limites de trama 
MAX_DIGITOS_COMPRA = 10
MAX_DIGITOS_PRODUCTO = 10
MAX_DIGITOS_CANTIDAD = 7
#numeros de compra en hilos 
_en_proceso = set()
_candado = threading.Lock()

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
        
def _limpiar_productos(lista):
    """Lista de tuplas en texto, regresa none si falta algo"""
    if not isinstance(lista, list) or not lista:
        return None
    productos = []
    for item in lista: 
        if not isinstance(item, dict):
            return None 
        codigo = item.get("codigo_producto")
        cantidad = item.get("cantidad")
        if not (val.es_texto(codigo) and val.es_texto(cantidad)):
            return None
        productos.append((codigo.strip(), cantidad.strip()))
    return productos

def _limpiar(datos): 
    """Revisa que esten todos los campos y convierte fecha y total, regresa un diccionario listo para usar"""
    if not isinstance(datos, dict):
        return None
    
    compra = {}
    for campo in CAMPOS:
        valor = datos.get(campo)
        if not val.es_texto(valor):
            return None
        compra[campo] = valor.strip()
        
    compra["fecha"] = val.leer_fecha_de_hoy(compra["fecha_compra"])
    compra["total"] = val.leer_monto(compra["total_compra"])
    compra["productos"] = _limpiar_productos(datos.get("productos"))
    
    if None in (compra["fecha"], compra["total"], compra["productos"]):
        return None
    return compra

def _datos_validos(c):
    """reglas para datos"""
    if not val.entero_positivo(c["numero_compra"], MAX_DIGITOS_COMPRA):
        return False
    for campo in CAMPOS_CIFRADOS:
        if not val.parece_cifrado(c[campo]):
            return False
    for codigo,cantidad in c["productos"]:
        if not (val.entero_positivo(codigo, MAX_DIGITOS_PRODUCTO)
                and val.entero_positivo(cantidad, MAX_DIGITOS_CANTIDAD)):
            return False
    return True
    
def _registrar(c):
    """validaciones con la base, consulta al almacen y guardado"""
    if DB.existe_compra(c["numero_compra"]) or not DB.existe_cliente(c["id_cliente"]):
        return ERROR_VALIDACION
    
    trama = Almacen.armar_trama_salida(c["numero_compra"], c["fecha_compra"], c["productos"])
    respuesta = Almacen.enviar(trama)
    bitacora.registrar(respuesta, origen="Almacen")
    
    if respuesta != Almacen.EXITOSO:
        print(f"[VERIFICADOR2] EL almacen rechazo la compra {c['numero_compra']}: {respuesta}")
        return ERROR_PROCESO
    
    productos = [(codigo, int(cantidad)) for codigo, cantidad in c["productos"]]
    DB.registrar_compra(c["numero_compra"], c["id_cliente"], c["fecha"], c["total"], 
                       c["num_tarjeta"], c["fecha_vencimiento"], c["codigo_verificacion"], productos)
    return EXITO

def procesar(datos):
    """recibe el diccionario de la trama y devuelve el diccionario de respuesta"""
    try: 
        compra = _limpiar(datos)
        if compra is None or not _datos_validos(compra):
            return ERROR_VALIDACION
        numero = compra["numero_compra"]
        if not _reservar(numero):
            return ERROR_VALIDACION
        try: 
            return _registrar(compra)
        finally: 
            _liberar(numero)
            
    except Exception as error:
        print(f"[VERIFICADOR2] Error al procesar la compra: {error}")
        traceback.print_exc()
        return ERROR_PROCESO        