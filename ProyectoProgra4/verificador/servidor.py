#recibe tramas por sockets, atiende a cada cliente con su hilo propio, 
#registra en bitacora y responde 

import socket
import threading
import json
from Logica import Verificador1
from Logica import Verificador2

from config import PUERTO
from Logica import Verificador3 as bitacora 

HOST= "0.0.0.0"
TAMANO_BUFFER = 65536

def main(): 
    bitacora.iniciar() # arranca el hilo
    servidor = socket.socket(socket.AF_INET, socket.SOCK_STREAM) #crea el socket 
    servidor.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR, 1) #reinciar puerto al iniciar
    servidor.bind((HOST, PUERTO)) #juntar socket a puerto 
    servidor.listen() #socket en modo espera 
    servidor.settimeout(1.0)     
    print(f"Verificador escuchando en el puerto {PUERTO}...")
    
    try: 
        while True:
            try:
                conexion, direccion = servidor.accept()
            except socket.timeout: 
                continue
            conexion.settimeout(10)
            hilo = threading.Thread(target=atender_cliente, args=(conexion, direccion), daemon=True)
            hilo.start()
    except KeyboardInterrupt:
        print("Apagando el servidor..")
    finally: 
        servidor.close()
        bitacora.detener()
    
def atender_cliente(conexion, direccion): 
    with conexion: 
        try: 
            datos = conexion.recv(TAMANO_BUFFER)
            if not datos: 
                return
            trama = datos.decode("utf-8")
            bitacora.registrar(trama, origen=direccion[0])
            operacion, respuesta_dict = _procesar(trama)
            respuesta = json.dumps(respuesta_dict, ensure_ascii=False)
            bitacora.registrar_resultado(operacion, respuesta_dict, destino=direccion[0])
            conexion.sendall(respuesta.encode("utf-8"))
        except (OSError, UnicodeDecodeError) as error:
            print(f"Error atendiendo a {direccion}: {error}")

#marcador temporal 
def _procesar(trama):
    """Convierte la trama a diccionario, la envia a la historia que corresponde
    y devuelve (operacion, respuesta) para registrar el resultado en la bitacora"""
    try:
        datos = json.loads(trama)
    except json.JSONDecodeError:
        return "desconocida", {"status": "1", "errores": [
            {"campo": "trama", "detalle": "La trama no es un JSON válido."}]}

    if not isinstance(datos, dict) or "transaccion" not in datos:
        return "desconocida", {"status": "1", "errores": [
            {"campo": "transaccion", "detalle": "Falta el tipo de transacción."}]}

    if datos.get("transaccion") == "compra":
        return "compra", Verificador2.procesar(datos)
    return "cliente_" + str(datos.get("transaccion", "")).strip().lower(), Verificador1.procesar(datos)

def procesar(trama):
    """Se mantiene por compatibilidad: devuelve la respuesta ya en texto JSON"""
    return json.dumps(_procesar(trama)[1], ensure_ascii=False)

if __name__ == "__main__":
    main()