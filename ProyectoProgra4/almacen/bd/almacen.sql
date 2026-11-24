-- =====================================================================
-- Script completo de Base de Datos: NovaOnlineAlmacen
-- Historias: ALMACEN1, ALMACEN2, ALMACEN3, ALMACEN4
-- =====================================================================

-- Creacion condicional de la base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NovaOnlineAlmacen')
BEGIN
    CREATE DATABASE NovaOnlineAlmacen;
END
GO

USE NovaOnlineAlmacen;
GO

-- =====================================================================
-- TABLAS (se crean una sola vez por entorno nuevo)
-- =====================================================================

-- Catalogo maestro de productos (codigo exacto de 10 digitos numericos)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[producto]') AND type IN (N'U'))
CREATE TABLE dbo.producto (
    no_producto  VARCHAR(10)     NOT NULL,
    nombre       VARCHAR(90)     NOT NULL,
    precio       DECIMAL(8,2)    NOT NULL,
    CONSTRAINT PK_producto PRIMARY KEY (no_producto)
);
GO

-- Existencias actuales por producto
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[inventario]') AND type IN (N'U'))
CREATE TABLE dbo.inventario (
    no_producto      VARCHAR(10)  NOT NULL,
    cantidad_actual  INT          NOT NULL DEFAULT 0,
    CONSTRAINT PK_inventario PRIMARY KEY (no_producto),
    CONSTRAINT FK_inventario_producto FOREIGN KEY (no_producto)
        REFERENCES dbo.producto (no_producto)
);
GO

-- Catalogo de proveedores (estado: 1 = Activo, 2 = Inactivo)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proveedor]') AND type IN (N'U'))
CREATE TABLE dbo.proveedor (
    cedula_juridica  VARCHAR(10)     NOT NULL,
    nombre           VARCHAR(100)    NOT NULL,
    nombre_contacto  VARCHAR(75)     NOT NULL,
    telefono         VARCHAR(8)      NOT NULL,
    correo           VARCHAR(75)     NOT NULL,
    estado           CHAR(1)         NOT NULL,
    CONSTRAINT PK_proveedor PRIMARY KEY (cedula_juridica),
    CONSTRAINT CK_proveedor_estado CHECK (estado IN ('1','2'))
);
GO

-- Encabezado de compras / ingresos a almacen
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[compra]') AND type IN (N'U'))
CREATE TABLE dbo.compra (
    numero_ingreso   BIGINT          NOT NULL,
    fecha_compra     DATE            NOT NULL,
    cedula_juridica  VARCHAR(10)     NOT NULL,
    CONSTRAINT PK_compra PRIMARY KEY (numero_ingreso),
    CONSTRAINT FK_compra_proveedor FOREIGN KEY (cedula_juridica)
        REFERENCES dbo.proveedor (cedula_juridica)
);
GO

-- Detalle de productos ingresados por cada compra
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[compra_detalle]') AND type IN (N'U'))
CREATE TABLE dbo.compra_detalle (
    id               INT IDENTITY(1,1) NOT NULL,
    numero_ingreso   BIGINT          NOT NULL,
    no_producto      VARCHAR(10)     NOT NULL,
    cantidad         INT             NOT NULL,
    CONSTRAINT PK_compra_detalle PRIMARY KEY (id),
    CONSTRAINT FK_compradet_compra FOREIGN KEY (numero_ingreso)
        REFERENCES dbo.compra (numero_ingreso),
    CONSTRAINT FK_compradet_producto FOREIGN KEY (no_producto)
        REFERENCES dbo.producto (no_producto),
    CONSTRAINT CK_compradet_cantidad CHECK (cantidad > 0)
);
GO

-- Historico de productos que cada proveedor le ha vendido al almacen
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proveedor_producto]') AND type IN (N'U'))
CREATE TABLE dbo.proveedor_producto (
    cedula_juridica       VARCHAR(10)  NOT NULL,
    no_producto           VARCHAR(10)  NOT NULL,
    fecha_primera_compra  DATE         NOT NULL,
    CONSTRAINT PK_proveedor_producto PRIMARY KEY (cedula_juridica, no_producto),
    CONSTRAINT FK_provprod_proveedor FOREIGN KEY (cedula_juridica)
        REFERENCES dbo.proveedor (cedula_juridica),
    CONSTRAINT FK_provprod_producto FOREIGN KEY (no_producto)
        REFERENCES dbo.producto (no_producto)
);
GO

-- Encabezado de salidas de inventario por ventas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[salida]') AND type IN (N'U'))
CREATE TABLE dbo.salida (
    numero_factura   BIGINT          NOT NULL,
    fecha_venta      DATE            NOT NULL,
    CONSTRAINT PK_salida PRIMARY KEY (numero_factura)
);
GO

-- Detalle de productos descontados por cada salida
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[salida_detalle]') AND type IN (N'U'))
CREATE TABLE dbo.salida_detalle (
    id               INT IDENTITY(1,1) NOT NULL,
    numero_factura   BIGINT          NOT NULL,
    no_producto      VARCHAR(10)     NOT NULL,
    cantidad         INT             NOT NULL,
    CONSTRAINT PK_salida_detalle PRIMARY KEY (id),
    CONSTRAINT FK_salidadet_salida FOREIGN KEY (numero_factura)
        REFERENCES dbo.salida (numero_factura),
    CONSTRAINT FK_salidadet_producto FOREIGN KEY (no_producto)
        REFERENCES dbo.producto (no_producto),
    CONSTRAINT CK_salidadet_cantidad CHECK (cantidad > 0)
);
GO

-- =====================================================================
-- TRIGGERS
-- =====================================================================

-- Crea registro inicial en cero en inventario al registrar un producto nuevo
IF OBJECT_ID('dbo.TR_producto_insert_inventario', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_producto_insert_inventario;
GO

CREATE TRIGGER dbo.TR_producto_insert_inventario
ON dbo.producto
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.inventario (no_producto, cantidad_actual)
    SELECT no_producto, 0
    FROM inserted;
END;
GO

-- Suma automaticamente al inventario cuando se inserta una linea de compra
IF OBJECT_ID('dbo.TR_compra_detalle_sube_inventario', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_compra_detalle_sube_inventario;
GO

CREATE TRIGGER dbo.TR_compra_detalle_sube_inventario
ON dbo.compra_detalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE inv
    SET inv.cantidad_actual = inv.cantidad_actual + i.cantidad
    FROM dbo.inventario inv
    INNER JOIN inserted i ON inv.no_producto = i.no_producto;
END;
GO

-- Enlaza el producto con el proveedor en la primera compra registrada
IF OBJECT_ID('dbo.TR_compra_detalle_registra_producto_proveedor', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_compra_detalle_registra_producto_proveedor;
GO

CREATE TRIGGER dbo.TR_compra_detalle_registra_producto_proveedor
ON dbo.compra_detalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.proveedor_producto (cedula_juridica, no_producto, fecha_primera_compra)
    SELECT c.cedula_juridica, i.no_producto, MIN(c.fecha_compra)
    FROM inserted i
    INNER JOIN dbo.compra c ON c.numero_ingreso = i.numero_ingreso
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.proveedor_producto pp
        WHERE pp.cedula_juridica = c.cedula_juridica
          AND pp.no_producto = i.no_producto
    )
    GROUP BY c.cedula_juridica, i.no_producto;
END;
GO

-- Descuenta del inventario al insertar detalle de salida y bloquea saldos negativos
IF OBJECT_ID('dbo.TR_salida_detalle_baja_inventario', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_salida_detalle_baja_inventario;
GO

CREATE TRIGGER dbo.TR_salida_detalle_baja_inventario
ON dbo.salida_detalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM dbo.inventario inv
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
    FROM dbo.inventario inv
    INNER JOIN inserted i ON inv.no_producto = i.no_producto;
END;
GO

-- =====================================================================
-- PROCEDIMIENTOS ALMACENADOS
-- =====================================================================

-- ALMACEN1 - Ingreso de producto (exige exactamente 10 digitos numericos)
IF OBJECT_ID('dbo.sp_ingresar_producto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ingresar_producto;
GO

CREATE PROCEDURE dbo.sp_ingresar_producto
    @no_producto VARCHAR(10),
    @nombre VARCHAR(90),
    @precio DECIMAL(8,2),
    @resultado VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Valida longitud exacta de 10 digitos numericos
        IF (@no_producto IS NULL OR LEN(@no_producto) <> 10 OR @no_producto LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de producto: debe tener exactamente 10 digitos numericos.';
            RETURN;
        END

        -- Valida que el nombre no este vacio ni sea puramente numerico
        IF (@nombre IS NULL OR LTRIM(RTRIM(@nombre)) = '' OR @nombre NOT LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: nombre del producto: es obligatorio y no puede ser solo numeros.';
            RETURN;
        END

        -- Valida precio positivo
        IF (@precio IS NULL OR @precio <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO: precio: debe ser mayor a cero.';
            RETURN;
        END

        -- Verifica duplicidad de codigo
        IF EXISTS (SELECT 1 FROM dbo.producto WHERE no_producto = @no_producto)
        BEGIN
            SET @resultado = 'DUPLICADO';
            RETURN;
        END

        INSERT INTO dbo.producto (no_producto, nombre, precio)
        VALUES (@no_producto, @nombre, @precio);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN1 - Modificar producto existente
IF OBJECT_ID('dbo.sp_modificar_producto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_modificar_producto;
GO

CREATE PROCEDURE dbo.sp_modificar_producto
    @no_producto VARCHAR(10),
    @nombre VARCHAR(90),
    @precio DECIMAL(8,2),
    @resultado VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Valida longitud exacta de 10 digitos numericos
        IF (@no_producto IS NULL OR LEN(@no_producto) <> 10 OR @no_producto LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de producto: debe tener exactamente 10 digitos numericos.';
            RETURN;
        END

        -- Verifica existencia del producto
        IF NOT EXISTS (SELECT 1 FROM dbo.producto WHERE no_producto = @no_producto)
        BEGIN
            SET @resultado = 'PRODUCTO INVALIDO';
            RETURN;
        END

        -- Valida nombre obligatorio y no puramente numerico
        IF (@nombre IS NULL OR LTRIM(RTRIM(@nombre)) = '' OR @nombre NOT LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: nombre del producto: es obligatorio y no puede ser solo numeros.';
            RETURN;
        END

        -- Valida precio positivo
        IF (@precio IS NULL OR @precio <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO: precio: debe ser mayor a cero.';
            RETURN;
        END

        UPDATE dbo.producto
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

-- ALMACEN2 - Mantenimiento de proveedor (3 = Ingreso, 4 = Modificacion)
IF OBJECT_ID('dbo.sp_proveedor_mantenimiento', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_proveedor_mantenimiento;
GO

CREATE PROCEDURE dbo.sp_proveedor_mantenimiento
    @tipoTransaccion   CHAR(1),
    @cedulaJuridica    VARCHAR(10),
    @nombre            VARCHAR(100),
    @nombreContacto    VARCHAR(75),
    @telefono          VARCHAR(8),
    @correo            VARCHAR(75),
    @estado            CHAR(1),
    @resultado         VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Valida cedula juridica de 10 digitos numericos
        IF (@cedulaJuridica IS NULL OR LEN(@cedulaJuridica) <> 10 OR @cedulaJuridica LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: cedula juridica: debe tener exactamente 10 digitos numericos.';
            RETURN;
        END

        -- Valida nombre de la empresa
        IF (@nombre IS NULL OR LTRIM(RTRIM(@nombre)) = '' OR @nombre NOT LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: nombre de la empresa: es obligatorio y no puede ser solo numeros.';
            RETURN;
        END

        -- Valida nombre del contacto
        IF (@nombreContacto IS NULL OR LTRIM(RTRIM(@nombreContacto)) = '' OR @nombreContacto NOT LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: nombre del contacto: es obligatorio y no puede ser solo numeros.';
            RETURN;
        END

        -- Valida telefono de 8 digitos numericos
        IF (@telefono IS NULL OR LEN(@telefono) <> 8 OR @telefono LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: telefono: debe tener exactamente 8 digitos numericos.';
            RETURN;
        END

        -- Valida correo obligatorio
        IF (@correo IS NULL OR LTRIM(RTRIM(@correo)) = '')
        BEGIN
            SET @resultado = 'DATO INVALIDO: correo: es obligatorio.';
            RETURN;
        END

        -- Valida estado permitido (1 = Activo, 2 = Inactivo)
        IF (@estado NOT IN ('1','2'))
        BEGIN
            SET @resultado = 'DATO INVALIDO: estado: debe ser 1 o 2.';
            RETURN;
        END

        IF @tipoTransaccion = '3' -- Ingreso
        BEGIN
            IF @estado <> '1'
            BEGIN
                SET @resultado = 'DATO INVALIDO: estado: al ingresar un proveedor debe ser 1 (activo).';
                RETURN;
            END

            IF EXISTS (SELECT 1 FROM dbo.proveedor WHERE cedula_juridica = @cedulaJuridica)
            BEGIN
                SET @resultado = 'DUPLICADO';
                RETURN;
            END

            IF EXISTS (SELECT 1 FROM dbo.proveedor WHERE nombre = LTRIM(RTRIM(@nombre)))
            BEGIN
                SET @resultado = 'DATO INVALIDO: nombre de la empresa: ya existe un proveedor con ese nombre.';
                RETURN;
            END

            INSERT INTO dbo.proveedor (cedula_juridica, nombre, nombre_contacto, telefono, correo, estado)
            VALUES (@cedulaJuridica, @nombre, @nombreContacto, @telefono, @correo, @estado);

            SET @resultado = 'EXITOSO';
        END
        ELSE IF @tipoTransaccion = '4' -- Modificacion
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.proveedor WHERE cedula_juridica = @cedulaJuridica)
            BEGIN
                SET @resultado = 'PROVEEDOR INVALIDO';
                RETURN;
            END

            IF EXISTS (SELECT 1 FROM dbo.proveedor
                       WHERE nombre = LTRIM(RTRIM(@nombre))
                         AND cedula_juridica <> @cedulaJuridica)
            BEGIN
                SET @resultado = 'DATO INVALIDO: nombre de la empresa: ya existe otro proveedor con ese nombre.';
                RETURN;
            END

            UPDATE dbo.proveedor
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
            SET @resultado = 'DATO INVALIDO: tipo de transaccion: debe ser 3 o 4.';
        END
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN3 - Registrar encabezado de compra a proveedor
IF OBJECT_ID('dbo.sp_compra_registrar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_compra_registrar;
GO

CREATE PROCEDURE dbo.sp_compra_registrar
    @numeroIngreso     BIGINT,
    @fechaCompra       DATE,
    @cedulaJuridica    VARCHAR(10),
    @resultado         VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Valida numero de ingreso positivo
        IF (@numeroIngreso IS NULL OR @numeroIngreso <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de ingreso: debe ser mayor a cero.';
            RETURN;
        END

        -- Valida fecha no nula y no posterior a hoy
        IF (@fechaCompra IS NULL OR @fechaCompra > CAST(GETDATE() AS DATE))
        BEGIN
            SET @resultado = 'DATO INVALIDO: fecha de compra: no puede ser mayor a la fecha actual.';
            RETURN;
        END

        -- Verifica existencia del proveedor
        IF NOT EXISTS (SELECT 1 FROM dbo.proveedor WHERE cedula_juridica = @cedulaJuridica)
        BEGIN
            SET @resultado = 'PROVEEDOR INVALIDO';
            RETURN;
        END

        -- Verifica que el numero de ingreso no este repetido
        IF EXISTS (SELECT 1 FROM dbo.compra WHERE numero_ingreso = @numeroIngreso)
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de ingreso: ya existe una compra con ese numero.';
            RETURN;
        END

        INSERT INTO dbo.compra (numero_ingreso, fecha_compra, cedula_juridica)
        VALUES (@numeroIngreso, @fechaCompra, @cedulaJuridica);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN3 - Agregar linea de detalle de compra
IF OBJECT_ID('dbo.sp_compra_detalle_agregar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_compra_detalle_agregar;
GO

CREATE PROCEDURE dbo.sp_compra_detalle_agregar
    @numeroIngreso     BIGINT,
    @noProducto        VARCHAR(10),
    @cantidad          INT,
    @resultado         VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Valida numero de producto de exactamente 10 digitos numericos
        IF (@noProducto IS NULL OR LEN(@noProducto) <> 10 OR @noProducto LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de producto: debe tener exactamente 10 digitos numericos.';
            RETURN;
        END

        -- Valida cantidad positiva
        IF (@cantidad IS NULL OR @cantidad <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO: cantidad: debe ser mayor a cero.';
            RETURN;
        END

        -- Verifica existencia del producto
        IF NOT EXISTS (SELECT 1 FROM dbo.producto WHERE no_producto = @noProducto)
        BEGIN
            SET @resultado = 'PRODUCTO INVALIDO';
            RETURN;
        END

        INSERT INTO dbo.compra_detalle (numero_ingreso, no_producto, cantidad)
        VALUES (@numeroIngreso, @noProducto, @cantidad);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN4 - Registrar encabezado de salida por venta
IF OBJECT_ID('dbo.sp_salida_registrar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_salida_registrar;
GO

CREATE PROCEDURE dbo.sp_salida_registrar
    @numeroFactura     BIGINT,
    @fechaVenta        DATE,
    @resultado         VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';

    BEGIN TRY
        -- Valida numero de compra positivo
        IF (@numeroFactura IS NULL OR @numeroFactura <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de compra: debe ser mayor a cero.';
            RETURN;
        END

        -- Valida fecha no nula y no posterior a hoy
        IF (@fechaVenta IS NULL OR @fechaVenta > CAST(GETDATE() AS DATE))
        BEGIN
            SET @resultado = 'DATO INVALIDO: fecha de compra: no puede ser mayor a la fecha actual.';
            RETURN;
        END

        -- Verifica que el numero de compra no exista previamente
        IF EXISTS (SELECT 1 FROM dbo.salida WHERE numero_factura = @numeroFactura)
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de compra: ya existe una salida con ese numero.';
            RETURN;
        END

        INSERT INTO dbo.salida (numero_factura, fecha_venta)
        VALUES (@numeroFactura, @fechaVenta);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- ALMACEN4 - Agregar linea de detalle de salida y descontar inventario
IF OBJECT_ID('dbo.sp_salida_detalle_agregar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_salida_detalle_agregar;
GO

CREATE PROCEDURE dbo.sp_salida_detalle_agregar
    @numeroFactura     BIGINT,
    @noProducto        VARCHAR(10),
    @cantidad          INT,
    @resultado         VARCHAR(120) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @resultado = 'ERROR';
    DECLARE @cantidadActual INT;

    BEGIN TRY
        -- Valida numero de producto de exactamente 10 digitos numericos
        IF (@noProducto IS NULL OR LEN(@noProducto) <> 10 OR @noProducto LIKE '%[^0-9]%')
        BEGIN
            SET @resultado = 'DATO INVALIDO: numero de producto: debe tener exactamente 10 digitos numericos.';
            RETURN;
        END

        -- Valida cantidad positiva
        IF (@cantidad IS NULL OR @cantidad <= 0)
        BEGIN
            SET @resultado = 'DATO INVALIDO: cantidad: debe ser mayor a cero.';
            RETURN;
        END

        SELECT @cantidadActual = cantidad_actual
        FROM dbo.inventario
        WHERE no_producto = @noProducto;

        -- Verifica existencia del producto en inventario
        IF @cantidadActual IS NULL
        BEGIN
            SET @resultado = 'PRODUCTO INVALIDO';
            RETURN;
        END

        -- Verifica disponibilidad suficiente
        IF @cantidadActual < @cantidad
        BEGIN
            SET @resultado = 'CANTIDAD INSUF';
            RETURN;
        END

        INSERT INTO dbo.salida_detalle (numero_factura, no_producto, cantidad)
        VALUES (@numeroFactura, @noProducto, @cantidad);

        SET @resultado = 'EXITOSO';
    END TRY
    BEGIN CATCH
        SET @resultado = 'ERROR';
    END CATCH
END;
GO

-- =====================================================================
-- llena proveedor_producto con compras previas
-- =====================================================================
INSERT INTO dbo.proveedor_producto (cedula_juridica, no_producto, fecha_primera_compra)
SELECT c.cedula_juridica, cd.no_producto, MIN(c.fecha_compra)
FROM dbo.compra_detalle cd
INNER JOIN dbo.compra c ON c.numero_ingreso = cd.numero_ingreso
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.proveedor_producto pp
    WHERE pp.cedula_juridica = c.cedula_juridica
      AND pp.no_producto = cd.no_producto
)
GROUP BY c.cedula_juridica, cd.no_producto;
GO

-- Consulta de verificacion opcional (mantener comentada):
-- SELECT pr.nombre AS proveedor, pr.cedula_juridica, p.no_producto,
--        p.nombre AS producto, pp.fecha_primera_compra
-- FROM dbo.proveedor_producto pp
-- JOIN dbo.proveedor pr ON pr.cedula_juridica = pp.cedula_juridica
-- JOIN dbo.producto p   ON p.no_producto = pp.no_producto
-- ORDER BY pr.nombre, p.nombre;