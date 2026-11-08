#Verificador 3 Bitacora


import json
import queue
import threading
from datetime import datetime

ARCHIVO_BITACORA = "bitacora.txt"
FORMATO_FECHA = "%Y-%m-%d %H:%M:%S"

_cola = queue.Queue()
_FIN = None
_hilo = None

def registrar(trama, origen="simulador"):
    """Movimiento de entrada: trama que llega al verificador (o respuesta que llega del almacen)"""
    _encolar("entrada", "origen", origen, trama)

def registrar_salida(trama, destino="simulador"):
    """Movimiento de salida: respuesta que envia el verificador o trama que envia a otro componente"""
    _encolar("salida", "destino", destino, trama)

def _encolar(movimiento, clave, valor, trama):
    """Arma el registro con la fecha de primero y lo deja en la cola para el hilo"""
    registro = {
        "fecha" : datetime.now().strftime(FORMATO_FECHA),
        "movimiento" : movimiento,
        clave : valor,
        "trama" : _preparar_trama(trama),
    }
    _cola.put(registro)

def _preparar_trama(trama):
    """Si la trama llega en json se guarda, si llega en otro formato se guarda como texto"""
    try:
        return json.loads(trama)
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