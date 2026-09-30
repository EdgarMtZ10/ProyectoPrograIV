-- =====================================================================
-- Historias: ALMACEN1, ALMACEN2, ALMACEN3, ALMACEN4
-- =====================================================================

--Creacion Condicional de la Base de Datos 
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NovaOnlineAlmacen')
BEGIN
    CREATE DATABASE NovaOnlineAlmacen;
END
GO

USE NovaOnlineAlmacen;
GO

-- =====================================================================
-- TABLAS (Ejecutar una sola vez por entorno nuevo)
-- =====================================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[producto]') AND type in (N'U'))
CREATE TABLE producto (
    no_producto  VARCHAR(10)     NOT NULL,
    nombre       VARCHAR(90)     NOT NULL,
    precio       DECIMAL(8,2)    NOT NULL,
    CONSTRAINT PK_producto PRIMARY KEY (no_producto)
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[inventario]') AND type in (N'U'))
CREATE TABLE inventario (
    no_producto      VARCHAR(10)  NOT NULL,
    cantidad_actual  INT          NOT NULL DEFAULT 0,
    CONSTRAINT PK_inventario PRIMARY KEY (no_producto),
    CONSTRAINT FK_inventario_producto FOREIGN KEY (no_producto)
        REFERENCES producto (no_producto)
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proveedor]') AND type in (N'U'))
CREATE TABLE proveedor (
    cedula_juridica  VARCHAR(10)     NOT NULL,
    nombre             VARCHAR(100)    NOT NULL,
    nombre_contacto    VARCHAR(75)     NOT NULL,
    telefono           VARCHAR(8)      NOT NULL,
    correo             VARCHAR(75)     NOT NULL,
    estado             CHAR(1)         NOT NULL,
    CONSTRAINT PK_proveedor PRIMARY KEY (cedula_juridica),
    CONSTRAINT CK_proveedor_estado CHECK (estado IN ('1','2')) -- 1: Activo, 2: Inactivo
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[compra]') AND type in (N'U'))
CREATE TABLE compra (
    numero_ingreso     BIGINT          NOT NULL,
    fecha_compra       DATE            NOT NULL,
    cedula_juridica    VARCHAR(10)     NOT NULL,
    CONSTRAINT PK_compra PRIMARY KEY (numero_ingreso),
    CONSTRAINT FK_compra_proveedor FOREIGN KEY (cedula_juridica)
        REFERENCES proveedor (cedula_juridica)
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[compra_detalle]') AND type in (N'U'))
CREATE TABLE compra_detalle (
    id                 INT IDENTITY(1,1) NOT NULL,
    numero_ingreso     BIGINT          NOT NULL,
    no_producto        VARCHAR(10)     NOT NULL,
    cantidad           INT             NOT NULL,
    CONSTRAINT PK_compra_detalle PRIMARY KEY (id),
    CONSTRAINT FK_compradet_compra FOREIGN KEY (numero_ingreso)
        REFERENCES compra (numero_ingreso),
    CONSTRAINT FK_compradet_producto FOREIGN KEY (no_producto)
        REFERENCES producto (no_producto),
    CONSTRAINT CK_compradet_cantidad CHECK (cantidad > 0)
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[salida]') AND type in (N'U'))
CREATE TABLE salida (
    numero_factura     BIGINT          NOT NULL,
    fecha_venta        DATE            NOT NULL,
    CONSTRAINT PK_salida PRIMARY KEY (numero_factura)
);
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[salida_detalle]') AND type in (N'U'))
CREATE TABLE salida_detalle (
    id                 INT IDENTITY(1,1) NOT NULL,
    numero_factura     BIGINT          NOT NULL,
    no_producto        VARCHAR(10)     NOT NULL,
    cantidad           INT             NOT NULL,
    CONSTRAINT PK_salida_detalle PRIMARY KEY (id),
    CONSTRAINT FK_salidadet_salida FOREIGN KEY (numero_factura)
        REFERENCES salida (numero_factura),
    CONSTRAINT FK_salidadet_producto FOREIGN KEY (no_producto)
        REFERENCES producto (no_producto),
    CONSTRAINT CK_salidadet_cantidad CHECK (cantidad > 0)
);
GO


-- TRIGGERS


CREATE OR ALTER TRIGGER TR_compra_detalle_sube_inventario
ON compra_detalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE inv
    SET inv.cantidad_actual = inv.cantidad_actual + i.cantidad
    FROM inventario inv
    INNER JOIN inserted i ON inv.no_producto = i.no_producto;
END;
GO

CREATE OR ALTER TRIGGER TR_salida_detalle_baja_inventario
ON salida_detalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inventario inv
        INNER JOIN inserted i ON inv.no_producto = i.no_producto
        WHERE inv.cantidad_actual - i.cantidad < 0
    )
    BEGIN
        RAISERROR ('CANTIDAD INSUF', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE inv
    SET inv.cantidad_actual = inv.cantidad_actual - i.cantidad
    FROM inventario inv
    INNER JOIN inserted i ON inv.no_producto = i.no_producto;
END;
GO

-- =====================================================================
-- PROCEDIMIENTOS ALMACENADOS (CREATE OR ALTER para actualizar)
-- =====================================================================

-- ALMACEN1 - Modificar Producto
CREATE OR ALTER PROCEDURE sp_modificar_producto
    @no_producto VARCHAR(10),
    @nombre VARCHAR(90),
    @precio DECIMAL(8,2),
    @resultado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM producto WHERE no_producto = @no_producto)
        BEGIN
            SET @resultado = 'PRODUCTO INVALIDO';
            RETURN;
        END

        IF LEN(LTRIM(RTRIM(@nombre))) = 0
           OR @nombre NOT LIKE '%[^0-9]%'
           OR @precio <= 0
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        UPDATE producto
        SET nombre = @nombre,
            precio = @precio
        WHERE no_producto = @no_producto;

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN2 - Mantenimiento de Proveedor
CREATE OR ALTER PROCEDURE sp_proveedor_mantenimiento
    @tipoTransaccion   CHAR(1),
    @cedulaJuridica    VARCHAR(10),
    @nombre            VARCHAR(100),
    @nombreContacto    VARCHAR(75),
    @telefono          VARCHAR(8),
    @correo            VARCHAR(75),
    @estado            CHAR(1),
    @resultado         VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Validaciones de nulidad y vacíos
        IF (@cedulaJuridica IS NULL OR LTRIM(RTRIM(@cedulaJuridica)) = ''
            OR @nombre IS NULL OR LTRIM(RTRIM(@nombre)) = ''
            OR @nombreContacto IS NULL OR LTRIM(RTRIM(@nombreContacto)) = ''
            OR @estado NOT IN ('1','2'))
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        -- Validaciones estrictas del PDF
        IF (@cedulaJuridica LIKE '%[^0-9]%' 
            OR @nombre NOT LIKE '%[^0-9]%'  
            OR @nombreContacto NOT LIKE '%[^0-9]%') 
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        IF @tipoTransaccion = '3' -- Ingreso
        BEGIN
            IF @estado <> '1'
            BEGIN
                SET @resultado = 'DATO INVALIDO';
                RETURN;
            END
            
            IF EXISTS (SELECT 1 FROM proveedor WHERE cedula_juridica = @cedulaJuridica)
            BEGIN
                SET @resultado = 'DUPLICADO';
                RETURN;
            END

            INSERT INTO proveedor (cedula_juridica, nombre, nombre_contacto, telefono, correo, estado)
            VALUES (@cedulaJuridica, @nombre, @nombreContacto, @telefono, @correo, @estado);

            SET @resultado = 'EXITOSO';
        END
        ELSE IF @tipoTransaccion = '4' -- Modificacion
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM proveedor WHERE cedula_juridica = @cedulaJuridica)
            BEGIN
                SET @resultado = 'PROVEEDOR INVALIDO';
                RETURN;
            END

            UPDATE proveedor
            SET nombre = @nombre,
                nombre_contacto = @nombreContacto,
                telefono = @telefono,
                correo = @correo,
                estado = @estado
            WHERE cedula_juridica = @cedulaJuridica;

            SET @resultado = 'EXITOSO';
        END
        ELSE
        BEGIN
            SET @resultado = 'DATO INVALIDO';
        END
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN3 - sp_compra_registrar 
CREATE OR ALTER PROCEDURE sp_compra_registrar
    @numeroIngreso     BIGINT,
    @fechaCompra       DATE,
    @cedulaJuridica    VARCHAR(10),
    @resultado         VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        IF (@numeroIngreso IS NULL OR @numeroIngreso <= 0 OR @fechaCompra > CAST(GETDATE() AS DATE))
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM proveedor WHERE cedula_juridica = @cedulaJuridica)
        BEGIN
            SET @resultado = 'PROVEEDOR INVALIDO';
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM compra WHERE numero_ingreso = @numeroIngreso)
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        INSERT INTO compra (numero_ingreso, fecha_compra, cedula_juridica)
        VALUES (@numeroIngreso, @fechaCompra, @cedulaJuridica);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN3 - sp_compra_detalle_agregar 
CREATE OR ALTER PROCEDURE sp_compra_detalle_agregar
    @numeroIngreso     BIGINT,
    @noProducto        VARCHAR(10),
    @cantidad          INT,
    @resultado         VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        IF (@noProducto IS NULL OR @cantidad IS NULL OR @cantidad <= 0 OR @noProducto LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM producto WHERE no_producto = @noProducto)
        BEGIN
            SET @resultado = 'PRODUCTO INVALIDO';
            RETURN;
        END

        INSERT INTO compra_detalle (numero_ingreso, no_producto, cantidad)
        VALUES (@numeroIngreso, @noProducto, @cantidad);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN4 - sp_salida_registrar 
CREATE OR ALTER PROCEDURE sp_salida_registrar
    @numeroFactura     BIGINT,
    @fechaVenta        DATE,
    @resultado         VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        IF (@numeroFactura IS NULL OR @numeroFactura <= 0 OR @fechaVenta > CAST(GETDATE() AS DATE))
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM salida WHERE numero_factura = @numeroFactura)
        BEGIN
            SET @resultado = 'DATO INVALIDO'; 
            RETURN;
        END

        INSERT INTO salida (numero_factura, fecha_venta)
        VALUES (@numeroFactura, @fechaVenta);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

--
-- Trigger: crea inventario en cero al insertar producto
CREATE OR ALTER TRIGGER TR_producto_insert_inventario
ON producto
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO inventario (no_producto, cantidad_actual)
    SELECT no_producto, 0
    FROM inserted;
END;
GO

-- ALMACEN1 - Ingreso de producto
CREATE OR ALTER PROCEDURE sp_ingresar_producto
    @no_producto VARCHAR(10),
    @nombre VARCHAR(90),
    @precio DECIMAL(8,2),
    @resultado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        IF (@no_producto IS NULL OR @no_producto LIKE '%[^0-9]%'
            OR @nombre IS NULL OR LTRIM(RTRIM(@nombre)) = ''
            OR @nombre NOT LIKE '%[^0-9]%'
            OR @precio IS NULL OR @precio <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM producto WHERE no_producto = @no_producto)
        BEGIN
            SET @resultado = 'DUPLICADO';
            RETURN;
        END

        INSERT INTO producto (no_producto, nombre, precio)
        VALUES (@no_producto, @nombre, @precio);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO
--

-- ALMACEN4 - sp_salida_detalle_agregar 
CREATE OR ALTER PROCEDURE sp_salida_detalle_agregar
    @numeroFactura     BIGINT,
    @noProducto        VARCHAR(10),
    @cantidad          INT,
    @resultado         VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';
    DECLARE @cantidadActual INT;

    BEGIN TRY
        IF (@noProducto IS NULL OR @cantidad IS NULL OR @cantidad <= 0 OR @noProducto LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO';
            RETURN;
        END

        SELECT @cantidadActual = cantidad_actual
        FROM inventario
        WHERE no_producto = @noProducto;

        IF @cantidadActual IS NULL
        BEGIN
            SET @resultado = 'PRODUCTO INVALIDO';
            RETURN;
        END

        IF @cantidadActual < @cantidad
        BEGIN
            SET @resultado = 'CANTIDAD INSUF';
            RETURN;
        END

        INSERT INTO salida_detalle (numero_factura, no_producto, cantidad)
        VALUES (@numeroFactura, @noProducto, @cantidad);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO