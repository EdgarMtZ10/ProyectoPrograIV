using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;

namespace WsConsultasAlmacen
{
    // Repositorio de solo lectura contra SQL Server (NovaOnlineAlmacen)
    public class Repositorio
    {
        private readonly string cadenaConexion;

        // Obtiene la cadena de conexion configurada en Web.config
        public Repositorio()
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["NovaOnlineAlmacenConn"];
            cadenaConexion = setting != null ? setting.ConnectionString : string.Empty;
        }

        // Lista todos los productos con su existencia actual filtrando en todas las columnas
        public ProductoDto[] ListarProductos(string filtro)
        {
            ValidarFiltro(filtro);
            string patronLike = PrepararPatronLike(filtro);
            List<ProductoDto> lista = new List<ProductoDto>();

            // Consulta parametrizada con LEFT JOIN a inventario ordenada por nombre
            const string sql = @"
                SELECT p.no_producto,
                       p.nombre,
                       p.precio,
                       COALESCE(i.cantidad_actual, 0) AS cantidad_actual
                FROM producto p
                LEFT JOIN inventario i ON i.no_producto = p.no_producto
                WHERE (@filtro IS NULL
                       OR p.no_producto LIKE @patron ESCAPE '\'
                       OR p.nombre LIKE @patron ESCAPE '\'
                       OR CONVERT(VARCHAR(20), p.precio, 121) LIKE @patron ESCAPE '\'
                       OR CONVERT(VARCHAR(20), COALESCE(i.cantidad_actual, 0)) LIKE @patron ESCAPE '\')
                ORDER BY p.nombre ASC;";

            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Asigna parametros de filtro evitando concatenacion SQL
                    cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = (object)NormalizarFiltro(filtro) ?? DBNull.Value;
                    cmd.Parameters.Add("@patron", SqlDbType.NVarChar, 205).Value = (object)patronLike ?? DBNull.Value;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal precio = reader.GetDecimal(2);
                            int cantidad = reader.GetInt32(3);
                            lista.Add(new ProductoDto
                            {
                                NoProducto = reader.GetString(0),
                                Nombre = reader.GetString(1),
                                Precio = precio.ToString("0.00", CultureInfo.InvariantCulture),
                                Cantidad = cantidad.ToString(CultureInfo.InvariantCulture)
                            });
                        }
                    }
                }
                return lista.ToArray();
            }
            catch (SoapException)
            {
                throw;
            }
            catch (Exception)
            {
                // Lanza error SOAP generico sin exponer detalles internos de SQL
                throw new SoapException("No se pudo consultar productos.", SoapException.ServerFaultCode);
            }
        }

        // Busca un producto puntual por su codigo exacto de 10 digitos
        public ProductoDto ObtenerProducto(string noProducto)
        {
            ValidarNoProducto(noProducto);

            // Consulta puntual por llave primaria con existencia en inventario
            const string sql = @"
                SELECT p.no_producto,
                       p.nombre,
                       p.precio,
                       COALESCE(i.cantidad_actual, 0) AS cantidad_actual
                FROM producto p
                LEFT JOIN inventario i ON i.no_producto = p.no_producto
                WHERE p.no_producto = @noProducto;";

            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@noProducto", SqlDbType.VarChar, 10).Value = noProducto.Trim();

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Devuelve null si el producto no existe en la base
                        if (!reader.Read())
                        {
                            return null;
                        }

                        decimal precio = reader.GetDecimal(2);
                        int cantidad = reader.GetInt32(3);
                        return new ProductoDto
                        {
                            NoProducto = reader.GetString(0),
                            Nombre = reader.GetString(1),
                            Precio = precio.ToString("0.00", CultureInfo.InvariantCulture),
                            Cantidad = cantidad.ToString(CultureInfo.InvariantCulture)
                        };
                    }
                }
            }
            catch (SoapException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new SoapException("No se pudo consultar el producto.", SoapException.ServerFaultCode);
            }
        }

        // Lista proveedores aplicando filtro en cualquiera de sus columnas
        public ProveedorDto[] ListarProveedores(string filtro)
        {
            ValidarFiltro(filtro);
            string patronLike = PrepararPatronLike(filtro);
            List<ProveedorDto> lista = new List<ProveedorDto>();

            // Consulta parametrizada sobre tabla proveedor ordenada por nombre
            const string sql = @"
                SELECT cedula_juridica,
                       nombre,
                       nombre_contacto,
                       telefono,
                       correo,
                       estado
                FROM proveedor
                WHERE (@filtro IS NULL
                       OR cedula_juridica LIKE @patron ESCAPE '\'
                       OR nombre LIKE @patron ESCAPE '\'
                       OR nombre_contacto LIKE @patron ESCAPE '\'
                       OR telefono LIKE @patron ESCAPE '\'
                       OR correo LIKE @patron ESCAPE '\'
                       OR estado LIKE @patron ESCAPE '\')
                ORDER BY nombre ASC;";

            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = (object)NormalizarFiltro(filtro) ?? DBNull.Value;
                    cmd.Parameters.Add("@patron", SqlDbType.NVarChar, 205).Value = (object)patronLike ?? DBNull.Value;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ProveedorDto
                            {
                                CedulaJuridica = reader.GetString(0),
                                Nombre = reader.GetString(1),
                                NombreContacto = reader.GetString(2),
                                Telefono = reader.GetString(3),
                                Correo = reader.GetString(4),
                                Estado = reader.GetString(5)
                            });
                        }
                    }
                }
                return lista.ToArray();
            }
            catch (SoapException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new SoapException("No se pudo consultar proveedores.", SoapException.ServerFaultCode);
            }
        }

        // Obtiene un proveedor puntual por su cedula juridica de 10 digitos
        public ProveedorDto ObtenerProveedor(string cedulaJuridica)
        {
            ValidarCedulaJuridica(cedulaJuridica);

            // Consulta puntual por cedula juridica
            const string sql = @"
                SELECT cedula_juridica,
                       nombre,
                       nombre_contacto,
                       telefono,
                       correo,
                       estado
                FROM proveedor
                WHERE cedula_juridica = @cedulaJuridica;";

            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@cedulaJuridica", SqlDbType.VarChar, 10).Value = cedulaJuridica.Trim();

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Devuelve null si no se encuentra el proveedor
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new ProveedorDto
                        {
                            CedulaJuridica = reader.GetString(0),
                            Nombre = reader.GetString(1),
                            NombreContacto = reader.GetString(2),
                            Telefono = reader.GetString(3),
                            Correo = reader.GetString(4),
                            Estado = reader.GetString(5)
                        };
                    }
                }
            }
            catch (SoapException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new SoapException("No se pudo consultar el proveedor.", SoapException.ServerFaultCode);
            }
        }

        // Filtra compras por texto o fecha (dd/MM/yyyy o yyyyMMdd) y toma las 10 mas recientes
        public CompraResumenDto[] ListarUltimasCompras(string filtro)
        {
            ValidarFiltro(filtro);
            string normalizado = NormalizarFiltro(filtro);
            string patronLike = PrepararPatronLike(filtro);
            string fechaIso = ConvertirFiltroFechaAyyyyMMdd(normalizado);
            List<CompraResumenDto> lista = new List<CompraResumenDto>();

            // Aplica el filtro primero y luego limita a TOP 10 ordenado descendente
            const string sql = @"
                SELECT TOP (10)
                       c.numero_ingreso,
                       CONVERT(VARCHAR(8), c.fecha_compra, 112) AS fecha_yyyymmdd,
                       c.cedula_juridica,
                       pr.nombre AS nombre_empresa
                FROM compra c
                INNER JOIN proveedor pr ON pr.cedula_juridica = c.cedula_juridica
                WHERE (@filtro IS NULL
                       OR CONVERT(VARCHAR(30), c.numero_ingreso) LIKE @patron ESCAPE '\'
                       OR CONVERT(VARCHAR(8), c.fecha_compra, 112) LIKE @patron ESCAPE '\'
                       OR CONVERT(VARCHAR(10), c.fecha_compra, 103) LIKE @patron ESCAPE '\'
                       OR (@fechaIso IS NOT NULL AND CONVERT(VARCHAR(8), c.fecha_compra, 112) = @fechaIso)
                       OR c.cedula_juridica LIKE @patron ESCAPE '\'
                       OR pr.nombre LIKE @patron ESCAPE '\')
                ORDER BY c.fecha_compra DESC, c.numero_ingreso DESC;";

            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@filtro", SqlDbType.NVarChar, 100).Value = (object)normalizado ?? DBNull.Value;
                    cmd.Parameters.Add("@patron", SqlDbType.NVarChar, 205).Value = (object)patronLike ?? DBNull.Value;
                    cmd.Parameters.Add("@fechaIso", SqlDbType.VarChar, 8).Value = (object)fechaIso ?? DBNull.Value;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long numIngreso = reader.GetInt64(0);
                            lista.Add(new CompraResumenDto
                            {
                                NumeroIngreso = numIngreso.ToString(CultureInfo.InvariantCulture),
                                FechaCompra = reader.GetString(1),
                                CedulaJuridica = reader.GetString(2),
                                NombreEmpresa = reader.GetString(3)
                            });
                        }
                    }
                }
                return lista.ToArray();
            }
            catch (SoapException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new SoapException("No se pudo consultar las compras.", SoapException.ServerFaultCode);
            }
        }

        // Obtiene el encabezado y las lineas con nombre de producto de una compra
        public CompraDetalleDto ObtenerCompra(string numeroIngreso)
        {
            ValidarNumeroIngreso(numeroIngreso);
            long numIngresoValor = long.Parse(numeroIngreso.Trim(), CultureInfo.InvariantCulture);

            // Consulta de encabezado de compra unido con proveedor
            const string sqlEncabezado = @"
                SELECT c.numero_ingreso,
                       CONVERT(VARCHAR(8), c.fecha_compra, 112) AS fecha_yyyymmdd,
                       c.cedula_juridica,
                       pr.nombre AS nombre_empresa
                FROM compra c
                INNER JOIN proveedor pr ON pr.cedula_juridica = c.cedula_juridica
                WHERE c.numero_ingreso = @numeroIngreso;";

            // Consulta de lineas de detalle unidas con producto para obtener el nombre
            const string sqlLineas = @"
                SELECT cd.no_producto,
                       p.nombre,
                       cd.cantidad
                FROM compra_detalle cd
                INNER JOIN producto p ON p.no_producto = cd.no_producto
                WHERE cd.numero_ingreso = @numeroIngreso
                ORDER BY cd.id ASC;";

            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                {
                    conn.Open();

                    CompraDetalleDto cabecera = null;
                    using (SqlCommand cmdCab = new SqlCommand(sqlEncabezado, conn))
                    {
                        cmdCab.Parameters.Add("@numeroIngreso", SqlDbType.BigInt).Value = numIngresoValor;
                        using (SqlDataReader reader = cmdCab.ExecuteReader())
                        {
                            // Devuelve null si no existe el numero de ingreso
                            if (!reader.Read())
                            {
                                return null;
                            }

                            cabecera = new CompraDetalleDto
                            {
                                NumeroIngreso = reader.GetInt64(0).ToString(CultureInfo.InvariantCulture),
                                FechaCompra = reader.GetString(1),
                                CedulaJuridica = reader.GetString(2),
                                NombreEmpresa = reader.GetString(3),
                                Lineas = new LineaDto[0]
                            };
                        }
                    }

                    // Lee las lineas asociadas al ingreso encontrado
                    List<LineaDto> lineas = new List<LineaDto>();
                    using (SqlCommand cmdLin = new SqlCommand(sqlLineas, conn))
                    {
                        cmdLin.Parameters.Add("@numeroIngreso", SqlDbType.BigInt).Value = numIngresoValor;
                        using (SqlDataReader reader = cmdLin.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lineas.Add(new LineaDto
                                {
                                    Codigo = reader.GetString(0),
                                    Nombre = reader.GetString(1),
                                    Cantidad = reader.GetInt32(2).ToString(CultureInfo.InvariantCulture)
                                });
                            }
                        }
                    }

                    cabecera.Lineas = lineas.ToArray();
                    return cabecera;
                }
            }
            catch (SoapException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new SoapException("No se pudo consultar el detalle de la compra.", SoapException.ServerFaultCode);
            }
        }

        // Convierte filtro vacio o de espacios a null
        private static string NormalizarFiltro(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return null;
            }
            return filtro.Trim();
        }

        // Escapa los comodines de LIKE en SQL Server y envuelve en porcentajes
        private static string PrepararPatronLike(string filtro)
        {
            string norm = NormalizarFiltro(filtro);
            if (norm == null)
            {
                return null;
            }
            string escapado = norm
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_")
                .Replace("[", @"\[");
            return "%" + escapado + "%";
        }

        // Intenta convertir un filtro de fecha dd/MM/yyyy o yyyyMMdd a formato yyyyMMdd
        private static string ConvertirFiltroFechaAyyyyMMdd(string filtro)
        {
            if (string.IsNullOrEmpty(filtro))
            {
                return null;
            }
            DateTime fecha;
            if (DateTime.TryParseExact(filtro, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            }
            if (DateTime.TryParseExact(filtro, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            }
            return null;
        }

        // Valida que el numero de producto tenga exactamente 10 digitos numericos
        private static void ValidarNoProducto(string noProducto)
        {
            string valor = (noProducto ?? string.Empty).Trim();
            if (!Regex.IsMatch(valor, @"^\d{10}$"))
            {
                throw new SoapException("numero de producto: debe tener exactamente 10 digitos numericos.", SoapException.ClientFaultCode);
            }
        }

        // Valida que la cedula juridica tenga exactamente 10 digitos numericos
        private static void ValidarCedulaJuridica(string cedulaJuridica)
        {
            string valor = (cedulaJuridica ?? string.Empty).Trim();
            if (!Regex.IsMatch(valor, @"^\d{10}$"))
            {
                throw new SoapException("cedula juridica: debe tener exactamente 10 digitos numericos.", SoapException.ClientFaultCode);
            }
        }

        // Valida que el numero de ingreso sea obligatorio, numerico y maximo 10 digitos
        private static void ValidarNumeroIngreso(string numeroIngreso)
        {
            string valor = (numeroIngreso ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(valor) || valor.Length > 10 || !Regex.IsMatch(valor, @"^\d+$"))
            {
                throw new SoapException("numero de ingreso: es obligatorio, solo digitos y maximo 10 caracteres.", SoapException.ClientFaultCode);
            }
        }

        // Valida que el filtro no supere los 100 caracteres
        private static void ValidarFiltro(string filtro)
        {
            if (filtro != null && filtro.Length > 100)
            {
                throw new SoapException("filtro: maximo 100 caracteres.", SoapException.ClientFaultCode);
            }
        }
    }
}