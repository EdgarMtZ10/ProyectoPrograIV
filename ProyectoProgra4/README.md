GUIA DE EJECUCION DEL PROYECTO

1. Configurar las Bases de Datos (Solo la primera vez)
- SQL Server: Abra el archivo almacen\bd\almacen.sql en SQL Server Management Studio y ejecútelo presionando F5. Verifique que el usuario "sa" esté activo y el puerto TCP/IP 1433 esté habilitado.
- MySQL: Abra el archivo DataBase\SCRIPT_MYSQL_SISTEMAVERIFICADOR.sql en MySQL Workbench y ejecútelo presionando el botón del rayo.
- Contraseña: Abra el archivo verificador\config.py y escriba ahí su contraseña de MySQL.


2. Iniciar el Servidor del Almacén (Java)
Abra una terminal en la carpeta principal ProyectoProgra4.

Si necesita compilar (solo si hizo cambios en el código fuente), ejecute estos dos comandos:
Get-ChildItem -Recurse almacen -Filter *.java | Resolve-Path -Relative | Out-File -Encoding ascii fuentes.txt
javac -encoding UTF-8 -d bin -cp "lib\mssql-jdbc-13.6.0.jre11.jar" '@fuentes.txt'

Para ejecutar el servidor, corra este comando (recuerde cambiar CLAVE_SA por la contraseña real de su SQL Server):
java -cp "bin;lib\mssql-jdbc-13.6.0.jre11.jar" almacen.socket.Main 5000 localhost 1433 NovaOnlineAlmacen sa CLAVE_SA


3. Iniciar el Servidor Verificador (Python)
Abra una segunda terminal y entre a la carpeta del verificador:
cd verificador

Instale los requisitos (esto se hace solo la primera vez, no tiene que repetirlo):
pip install mysql-connector-python

Levante el servidor:
python servidor.py

python servidor.py
4. Ejecutar el Simulador (C#)
- Vaya a la carpeta ProyectoProgra4\simulador y haga doble clic en el archivo simulador.slnx para abrirlo en Visual Studio (si no abre, utilice simulador.csproj).
- Presione F5 para correr la interfaz gráfica.


5. Orden de uso recomendado
Para probar el flujo completo sin errores, registre los datos en el sistema siguiendo este orden:
1. Proveedor
2. Producto
3. Entrada de productos
4. Cliente
5. Compra

Nota: Para detener los servidores cuando termine de probar, vaya a cada terminal y presione Ctrl + C.