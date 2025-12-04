using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Globalization;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Loaders
{
    public class CsvLoader : ILoader
    {
        private readonly ILogger<CsvLoader> _logger;
        private readonly string _connectionString;

        public CsvLoader(ILogger<CsvLoader> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DWConnection")
                ?? throw new Exception("No se encontró 'DWConnection' en appsettings.json");
        }

        public OperationResult Load(object data)
        {
            try
            {
                _logger.LogInformation(" Iniciando carga hacia el Data Warehouse (CsvLoader)...");

                if (data == null)
                    return OperationResult.Failure("El paquete recibido por CsvLoader no es válido.");

                dynamic paquete = data;

                var clientes = paquete.Clientes as List<Clientes>;
                var productos = paquete.Productos as List<Productos>;
                var ventas = paquete.Ventas as List<Ventas>;

                if (clientes == null || productos == null || ventas == null)
                    return OperationResult.Failure("El paquete CSV no tiene la estructura esperada.");

                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                using var transaction = connection.BeginTransaction();

                
                var fuenteKey = InsertarDimFuenteDatos(connection, transaction);

                
                InsertarDimClientes(clientes, connection, transaction);

                
                InsertarDimProductos(productos, connection, transaction);

                
                InsertarDimFechasDesdeVentas(ventas, connection, transaction);

                transaction.Commit();

                _logger.LogInformation(" Carga de dimensiones completada correctamente.");
                return OperationResult.Success(null, "Carga CSV en Data Warehouse completada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error durante la carga en CsvLoader.");
                return OperationResult.Failure($"Error en CsvLoader: {ex.Message}");
            }
        }


        private int InsertarDimFuenteDatos(SqlConnection connection, SqlTransaction transaction)
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO Dimension.DimFuenteDatos (FuenteID, TipoFuente, FechaCarga)
                VALUES (@FuenteID, @TipoFuente, @FechaCarga);

                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction);

            cmd.Parameters.AddWithValue("@FuenteID", 1);                 
            cmd.Parameters.AddWithValue("@TipoFuente", "CSV");
            cmd.Parameters.AddWithValue("@FechaCarga", DateTime.Now);

            var result = cmd.ExecuteScalar();
            return (result is int fk) ? fk : 0;
        }

        private void InsertarDimClientes(
            List<Clientes> clientes,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            foreach (var c in clientes)
            {
                using var cmd = new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM Dimension.DimClientes WHERE ClienteID = @ClienteID)
                    BEGIN
                        INSERT INTO Dimension.DimClientes (ClienteID, Nombre, Email, Region)
                        VALUES (@ClienteID, @Nombre, @Email, @Region);
                    END;", connection, transaction);

                cmd.Parameters.AddWithValue("@ClienteID", c.IdCLiente);
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", c.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", c.Region ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertarDimProductos(
            List<Productos> productos,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            foreach (var p in productos)
            {
                using var cmd = new SqlCommand(@"
                    IF NOT EXISTS (SELECT 1 FROM Dimension.DimProductos WHERE ProductoID = @ProductoID)
                    BEGIN
                        INSERT INTO Dimension.DimProductos (ProductoID, Nombre, Categoria, Precio)
                        VALUES (@ProductoID, @Nombre, @Categoria, @Precio);
                    END;", connection, transaction);

                cmd.Parameters.AddWithValue("@ProductoID", p.IdProductos);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Categoria", p.Categoria ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", p.Precio);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertarDimFechasDesdeVentas(
    List<Ventas> ventas,
    SqlConnection connection,
    SqlTransaction transaction)
        {
            var cultura = new CultureInfo("es-DO");

            // Extraer fechas únicas desde ventas
            var fechasOriginales = ventas
                .Select(v => v.Fecha)
                .Distinct()
                .ToList();

            foreach (var fechaOriginal in fechasOriginales)
            {
                DateTime fechaReal;

                // 1️⃣ Validar que la fecha sea correcta
                if (fechaOriginal == DateTime.MinValue ||
                    fechaOriginal.Year < 1900 ||
                    fechaOriginal.Year > 2100)
                {
                    // 2️⃣ Asignar fecha por defecto
                    fechaReal = new DateTime(1900, 1, 1);
                }
                else
                {
                    fechaReal = fechaOriginal;
                }

                // 3️⃣ Generar FechaKey (YYYYMMDD)
                int fechaKey = int.Parse(fechaReal.ToString("yyyyMMdd"));

                // 4️⃣ Partes de la fecha
                int dia = fechaReal.Day;
                int mes = fechaReal.Month;
                int anio = fechaReal.Year;
                string nombreMes = cultura.DateTimeFormat.GetMonthName(mes);
                int trimestre = ((mes - 1) / 3) + 1;

                // 5️⃣ Insertar solo si no existe
                using var cmd = new SqlCommand(@"
            IF NOT EXISTS (SELECT 1 FROM Dimension.DimFecha WHERE FechaKey = @FechaKey)
            BEGIN
                INSERT INTO Dimension.DimFecha 
                (FechaKey, Fecha, Dia, Mes, NombreMes, Anio, Trimestre)
                VALUES 
                (@FechaKey, @Fecha, @Dia, @Mes, @NombreMes, @Anio, @Trimestre);
            END;", connection, transaction);

                cmd.Parameters.AddWithValue("@FechaKey", fechaKey);
                cmd.Parameters.AddWithValue("@Fecha", fechaReal);
                cmd.Parameters.AddWithValue("@Dia", dia);
                cmd.Parameters.AddWithValue("@Mes", mes);
                cmd.Parameters.AddWithValue("@NombreMes", nombreMes);
                cmd.Parameters.AddWithValue("@Anio", anio);
                cmd.Parameters.AddWithValue("@Trimestre", trimestre);

                cmd.ExecuteNonQuery();
            }
        }

    }
}
