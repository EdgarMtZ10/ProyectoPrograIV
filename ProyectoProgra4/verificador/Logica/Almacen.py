#comunicador del verificador con el almacen 
#arma la trama la envia por socket y devuelve la respuesta de almacen 

import socket
from config import ALMACEN_HOST, ALMACEN_PUERTO

TIPO_SALIDA = "6"
EXITOSO = "EXITOSO"

ANCHO_FACTURA = 10
ANCHO_FECHA = 10
ANCHO_PRODUCTO = 10
ANCHO_CANTIDAD = 7

TIMEOUT = 10
FIN_DE_TRAMA = "\n"

def armar_trama_salida(numero_factura, fecha, productos):
    """lista de tuplas validadas"""
    trama = (TIPO_SALIDA + numero_factura.zfill(ANCHO_FACTURA)
             + fecha.ljust(ANCHO_FECHA))
    for codigo, cantidad in productos:
        trama += codigo.zfill(ANCHO_PRODUCTO) + str(int(cantidad)).zfill(ANCHO_CANTIDAD)
    return trama

def _recibir_linea(conexion): 
    """lee hasta encontrar un salto de linea o el almacen cierre"""
    partes = []
    while True: 
        bloque = conexion.recv(1024)
        if not bloque:
            break
        partes.append(bloque)
        if b"\n" in bloque:
            break
    return b"".join(partes).decode("utf-8")
    
def enviar(trama):
    """envia la trama y devuelve la respuesta del almacen"""
    with socket.create_connection((ALMACEN_HOST, ALMACEN_PUERTO),
                timeout = TIMEOUT) as conexion:
        conexion.sendall((trama + FIN_DE_TRAMA).encode("utf-8"))
        respuesta = _recibir_linea(conexion)
    return respuesta.strip().upper()