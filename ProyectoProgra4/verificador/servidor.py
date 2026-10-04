#recibe tramas por sockets, atiende a cada cliente con su hilo propio, 
#registra en bitacora y responde 

import socket
import threading
import json
from Logica import Verificador1

from config import PUERTO
from Logica import Verificador3 as bitacora 

HOST= "0.0.0.0"
TAMANO_BUFFER = 4096

def main(): 
    bitacora.iniciar() # arranca el hilo
    servidor = socket.socket(socket.AF_INET, socket.SOCK_STREAM) #crea el socket 
    servidor.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR, 1) #reinciar puerto al iniciar
    servidor.bind((HOST, PUERTO)) #juntar socket a puerto 
    servidor.listen() #socket en modo espera 
    servidor.settimeout(1.0) 
    
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
            respuesta = procesar(trama)
            conexion.sendall(respuesta.encode("utf-8"))
        except (OSError, UnicodeDecodeError) as error:
            print(f"Error atendiendo a {direccion}: {error}")

#marcador temporal 
def procesar(trama): 
   """Convierte la trama a diccionario y la envia a la historia que corresponde"""
   try: 
       datos = json.loads(trama)
   except json.JSONDecodeError:
        return json.dumps({"status": "1"})
    
   if isinstance(datos, dict) and "transaccion" in datos:
        respuesta = Verificador1.procesar(datos)
   else: 
        respuesta = {"status": "1"}
   return json.dumps(respuesta, ensure_ascii=False)

if __name__ == "__main__":
    main()