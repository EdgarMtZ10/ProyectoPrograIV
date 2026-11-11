GUIA DE EJECUCION DEL PROYECTO

1. Configurar las Bases de Datos (Solo la primera vez)

SQL Server (Almacén): Abra el archivo almacen\bd\almacen.sql en SQL Server Management Studio y ejecútelo con F5. Verifique que el usuario "sa" esté activo y el puerto TCP/IP 1433 habilitado.

MySQL (Verificador):
Abra el archivo DataBase\SCRIPT_MYSQL_SISTEMAVERIFICADOR.sql en MySQL Workbench y ejecútelo presionando el botón del rayo.
Abra el archivo verificador\config.py y escriba ahí su contraseña real de MySQL.

MongoDB (Autenticación y Verificador): Asegúrese de tener el servicio de MongoDB local en ejecución (puerto 27017 por defecto). Si cuenta con un script o archivo JSON de respaldo, importe la colección inicial para tener al menos un usuario de prueba.

Configuración: Revise los archivos de configuración (como verificador\config.py o propiedades de Java) para colocar sus credenciales reales de bases de datos.

2. Iniciar los Servidores Backend (Sockets)
Servidor del Almacén (Java):
Abra una terminal en la carpeta principal ProyectoProgra4.

Para compilar (solo si hubo cambios):

PowerShell
Get-ChildItem -Recurse almacen -Filter *.java | Resolve-Path -Relative | Out-File -Encoding ascii fuentes.txt
javac -encoding UTF-8 -d bin -cp "lib\mssql-jdbc-13.6.0.jre11.jar" '@fuentes.txt'
Para ejecutar (cambie CLAVE_SA por su contraseña):

PowerShell
java -cp "bin;lib\mssql-jdbc-13.6.0.jre11.jar" almacen.socket.Main 5000 localhost 1433 NovaOnlineAlmacen sa CLAVE_SA
Servidor Verificador (Python):
Abra una segunda terminal y entre a la carpeta verificador.

Instale dependencias (incluyendo el driver de Mongo si es la primera vez):

PowerShell
pip install mysql-connector-python pymongo
Levante el servidor:

PowerShell
cd verificador
py servidor.py
3. Levantar los Web Services SOAP 
El simulador se comunica mediante servicios intermediarios (WsAlmacen y WsVerificador).

Abra las soluciones de los Web Services en Visual Studio y ejecútelos (F5) para que el servidor IIS Express los levante en sus respectivos puertos (localhost:5100 y localhost:5200).

Deje las pestañas del navegador abiertas en segundo plano.

*Nota de Seguridad: Si al correr los servicios Visual Studio arroja error porque Windows bloqueó las DLLs descargadas, vaya a la carpeta bin del servicio, elimínela por completo y haga clic en Recompilar Solución.*

4. Ejecutar el Simulador Gráfico (C#)
Vaya a la carpeta del cliente (ej. ProyectoProgra4\simulador2) y abra la solución en Visual Studio.

*Nota:(Para detener los servidores Java y Python, vaya a sus respectivas terminales y presione Ctrl + C).*