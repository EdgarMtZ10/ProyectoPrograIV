1. Ejecute el script actualizado en **SQL Server Management Studio (SSMS)** para crear o actualizar las tablas, triggers y procedimientos almacenados (incluye `@resultado VARCHAR(120)`, mensajes de error por campo sin tildes y validación de 10 dígitos exactos en `no_producto`).

2. Si ya tenía datos de pruebas anteriores, ejecute esta consulta para comprobar que no queden productos con códigos de largo distinto a 10 dígitos:

```sql
USE NovaOnlineAlmacen;
SELECT no_producto, LEN(no_producto) AS largo
FROM producto
WHERE LEN(no_producto) <> 10;
```

3. Si la consulta devuelve filas, ejecute el siguiente script para eliminar esos registros junto con sus dependencias en las tablas relacionadas y, posteriormente, vuelva a registrarlos con exactamente 10 dígitos numéricos (rellenando con ceros a la izquierda si es necesario) para evitar errores de `PRODUCTO INVALIDO` al comunicarse con el Verificador:

```sql
USE NovaOnlineAlmacen;
GO

BEGIN TRANSACTION;

DELETE FROM dbo.salida_detalle WHERE LEN(no_producto) <> 10;
DELETE FROM dbo.compra_detalle WHERE LEN(no_producto) <> 10;
DELETE FROM dbo.proveedor_producto WHERE LEN(no_producto) <> 10;
DELETE FROM dbo.inventario WHERE LEN(no_producto) <> 10;
DELETE FROM dbo.producto WHERE LEN(no_producto) <> 10;

COMMIT TRANSACTION;
GO
```