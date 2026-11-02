Mae de mi parte ya terminé el código de WsAutenticacion (1, 2 y 3). Está compilado, probado con casos buenos y malos, y subido a mi rama josue. Las 4 operaciones funcionan: Autenticar, CrearUsuario, ModificarUsuario y CambiarEstadoUsuario, con usuario y contraseña cifrados, MongoDB cargado y la bitácora JSON escribiendo cada transacción. Solo me falta mi parte de la documentación y una identificación real de cliente con Jean Carlo.

Lo que falta es conectarlo al simulador yo lo puedo hacer pero tendria que tocar el simulador pero lo que hay que hacer es:

1) Trae mi código: git fetch origin y git merge origin/josue. 
2) Carga la base: mongosh --file ProyectoProgra4\ws\WsAutenticacion\bd\autenticacion.js (debe decir: Usuarios en la colección: 2).
3) Corre mi servicio: abre ws\WsAutenticacion\WsAutenticacion.slnx y dale F5. Debe abrir http://localhost:5300/ServicioAutenticacion.asmx.
4) En simulador2 agrega la referencia de servicio igual que hiciste con WsAlmacen, con la URL http://localhost:5300/ServicioAutenticacion.asmx?WSDL
5) Crea la pantalla (login y mantenimiento de usuarios) que llame a las 4 operaciones. Todo va como string y en este orden:
- Autenticar(usuario, contrasena, tipoUsuario)
- CrearUsuario(identificacion, nombre, primerApellido, segundoApellido, correo, usuario, contrasena, estado, tipo)
- ModificarUsuario(identificacion, nombre, primerApellido, segundoApellido, correo, usuario, contrasena)
- CambiarEstadoUsuario(identificacion, estado)
6) Cifra usuario y contraseña con tu CifradoAes.Cifrar antes de enviar. Tipo: 1 empleado, 2 cliente. Estado: 1 activo, 2 inactivo.

Prueba rápida: usuario admin, contraseña Admin#Nova2026, tipo 1. Debe dar Resultado true, Id 101110111 y Mensaje Exitoso.

Cuando la tengas lista, avísame y la pruebo contra mi servicio. Si algo falla, lo ajusto de mi lado antes de la entrega.
