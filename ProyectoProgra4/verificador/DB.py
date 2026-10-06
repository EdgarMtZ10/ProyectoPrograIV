#Conexion de mysql a python 

import warnings
import mysql.connector
from config import DB_CONFIG

def obtener_conexion():
    return mysql.connector.connect(**DB_CONFIG)


##funciones internas 
def _leer_resultado(cursor): 
    """
    Lee el primer valor del select que devuelve un procedimiento 
    sacar el valor del select en los procedimientos 
    """
    with warnings.catch_warnings():
        warnings.simplefilter("ignore", DeprecationWarning)
        for resultado in cursor.stored_results():
            fila = resultado.fetchone()
            return fila[0] if fila else 0
        return 0
    
def _consultar(procedimiento, parametros):
    """
    Reciben el nombre del procedimiento y los parametros 
    de lectura      
    """
    conexion = obtener_conexion()
    try: 
        cursor = conexion.cursor()
        cursor.callproc(procedimiento, parametros)
        return _leer_resultado(cursor)
    finally: conexion.close()
    
def _ejectutar(procedimiento, parametros):
    """Recibe el nombre del procedimiento y parametros 
    para ejecutar los parametros, si es correcto realiza commit y 
    si algo falla un rollback para no ingresar datos incompletos en la BD"""
    conexion = obtener_conexion()
    try: 
        cursor = conexion.cursor()
        cursor.callproc(procedimiento, parametros)
        conexion.commit()
    except: 
        conexion.rollback()
        raise
        
    finally: conexion.close


def existe_cliente(id_cliente):
    return _consultar("SP_EXISTE_CLIENTE", [id_cliente]) == 1

def existe_compra(id_compra):
    return _consultar("SP_EXISTE_COMPRA", [id_compra]) > 0

def cliente_tiene_compras(id_cliente):
    return _consultar("SP_CLIENTE_TIENE_COMPRAS", [id_cliente]) == 1

def registrar_cliente(id_cliente, sig_pais, nombre, prim_apellido, seg_apellido,
                      correo, telefono, direccion):
    _ejectutar("SP_INSERTAR_CLIENTE", [id_cliente, sig_pais, nombre, prim_apellido, seg_apellido,
                      correo, telefono, direccion])

def modificar_cliente(id_cliente, sig_pais, nombre, prim_apellido, seg_apellido,
                      correo, telefono, direccion):
    _ejectutar("SP_MODIFICAR_CLIENTE", [id_cliente, sig_pais, nombre, prim_apellido, seg_apellido,
                      correo, telefono, direccion])  

def borrar_cliente(id_cliente):
    _ejectutar("SP_BORRAR_CLIENTE", [id_cliente])  

def registrar_compra(id_compra, id_cliente, fecha_compra, total_compra, num_tarjeta, 
                    fecha_vencimiento, codigo_verificacion, productos):
    """se registra compra y detalle en un solo proceso"""
    conexion = obtener_conexion()
    try: 
        cursor = conexion.cursor()
        cursor.callproc("SP_INSERTAR_COMPRA", [id_compra, id_cliente, fecha_compra, total_compra, num_tarjeta,
                                               fecha_vencimiento,codigo_verificacion])
        
        for id_producto, cantidad in productos:   # ahora sí itera sobre el parametro real
            cursor.callproc("SP_INSERTAR_DETALLE_COMPRA", (id_compra, id_producto, cantidad))
        
        conexion.commit() # si todo sale bien se guarda
    
    except Exception:
        conexion.rollback() #si falla se regresa todo y no se guarda nada 
        
    finally: 
        conexion.close()
