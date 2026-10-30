Funcionalidad Desarrollada

Se han completado y validado las historias de usuario correspondientes a la integración del Web Service del Almacén (Puerto 5100) y la actualización de las interfaces del Simulador v2.

Web Service Almacén (WsAlmacen)

Mantenimiento de Productos (WS_ALMACEN1): Recibe datos básicos y ensambla una trama de ancho fijo de 109 caracteres. Enruta la petición al socket del Almacén en Java usando el código de transacción 1 (ingreso) o 2 (modificación).

Mantenimiento de Proveedores (WS_ALMACEN2): Procesa 7 campos (incluyendo el estado 1 o 2) y ensambla una trama de 270 caracteres. Utiliza los códigos de transacción 3 (ingreso) o 4 (modificación).

Registro de Compras (WS_ALMACEN3): Recibe los datos de cabecera y una lista dinámica de productos. Ensambla una trama compuesta por una cabecera de 31 caracteres, más 17 caracteres adicionales por cada producto iterado. Código de transacción 5.

Nota: Todas las respuestas del socket (ej: EXITOSO, DUPLICADO) son interpretadas y devueltas al cliente en un objeto estandarizado con las propiedades Resultado (bool) y Mensaje (string).


Criptografía: Se integró la clase estática CifradoAes lista para usarse en el formulario "Pago de Compra" (Verificador).

Todos los campos numéricos se envían por SOAP como string para evitar que fallos de tipado generen errores HTTP 500 antes de alcanzar los métodos del servidor.

