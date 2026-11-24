#Verificador 3 Bitacora


import json
import queue
import threading
from datetime import datetime

ARCHIVO_BITACORA = "bitacora.txt"
FORMATO_FECHA = "%Y-%m-%d %H:%M:%S"
CAMPOS_OCULTOS = ("num_tarjeta", "fecha_vencimiento", "codigo_verificacion")
OCULTO = "[oculto]"

_cola = queue.Queue()
_FIN = None
_hilo = None

def registrar(trama, origen="simulador"):
    """Movimiento de entrada: trama que llega al verificador (o respuesta que llega del almacen)"""
    _encolar("entrada", "origen", origen, trama)


def registrar_resultado(operacion, respuesta, destino="simulador"):
    """Resultado final de un proceso: EXITOSO o ERROR, con el detalle por campo si hubo error"""
    exitoso = isinstance(respuesta, dict) and respuesta.get("status") == "OK"
    registro = {
        "fecha": datetime.now().strftime(FORMATO_FECHA),
        "movimiento": "resultado",
        "destino": destino,
        "operacion": operacion,
        "resultado": "EXITOSO" if exitoso else "ERROR",
        "respuesta": respuesta,
    }
    _cola.put(registro)
def _encolar(movimiento, clave, valor, trama):
    """Arma el registro con la fecha de primero y lo deja en la cola para el hilo"""
    registro = {
        "fecha" : datetime.now().strftime(FORMATO_FECHA),
        "movimiento" : movimiento,
        clave : valor,
        "trama" : _preparar_trama(trama),
    }
    _cola.put(registro)

def _ocultar(datos):
    """Reemplaza los datos de la tarjeta antes de guardar"""
    if isinstance(datos, dict):
        return {k: (OCULTO if k in CAMPOS_OCULTOS else _ocultar(v)) for k, v in datos.items()}
    if isinstance(datos, list):
        return [_ocultar(v) for v in datos]
    return datos

def _preparar_trama(trama):
    """Si la trama llega en json se guarda (sin datos de tarjeta), si llega en otro formato se guarda como texto"""
    if isinstance(trama, (dict, list)):
        return _ocultar(trama)
    try:
        return _ocultar(json.loads(trama))
    except (json.JSONDecodeError, TypeError):
        return trama


def _escritor():
    """Funcion que se usa dentro del hilo"""
    while True:
        registro = _cola.get()
        if registro is _FIN:
            _cola.task_done()
            break
        try:
            with open(ARCHIVO_BITACORA, "a", encoding="utf-8") as archivo:
                archivo.write(json.dumps(registro, ensure_ascii=False) + "\n")
        except OSError as error:
            print("Error al escribir la bitacora: ", error)
        finally:
            _cola.task_done()

def iniciar():
    """Arrancar el hilo"""
    global _hilo
    if _hilo is None or not _hilo.is_alive():
        _hilo = threading.Thread(target=_escritor, daemon=True)
        _hilo.start()

def detener():
    """Detener el hilo de forma ordenada"""
    global _hilo
    if _hilo is None or not _hilo.is_alive():
        return
    _cola.put(_FIN)
    _cola.join()
    _hilo = None