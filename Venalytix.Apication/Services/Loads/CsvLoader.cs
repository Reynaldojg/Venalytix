using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
                _logger.LogInformation("📥 Iniciando carga hacia el Data Warehouse (CsvLoader)...");

                if (data == null)
                    return OperationResult.Failure("CsvLoader recibió datos nulos.");

                // 🔵 Detectamos si el objeto es el paquete principal
                var paquete = data.GetType().GetProperty("Clientes") != null ? data : null;

                if (paquete == null)
                    return OperationResult.Failure("CsvLoader recibió un tipo NO válido.");

                // Extraer los datos del paquete
                var clientes = (List<Clientes>)paquete.GetType().GetProperty("Clientes").GetValue(paquete);
                var productos = (List<Productos>)paquete.GetType().GetProperty("Productos").GetValue(paquete);
                var ventas = (List<Ventas>)paquete.GetType().GetProperty("Ventas").GetValue(paquete);

                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                _logger.LogInformation("➡ Cargando DIM CLIENTES...");
                InsertarDimClientes(clientes, connection, transaction);

                _logger.LogInformation("➡ Cargando DIM PRODUCTOS...");
                InsertarDimProductos(productos, connection, transaction);

                _logger.LogInformation("➡ Cargando DIM FECHA...");
                InsertarDimFechasDesdeVentas(ventas, connection, transaction);

                transaction.Commit();

                _logger.LogInformation("✅ Dimensiones cargadas correctamente.");
                return OperationResult.Success("Dimensiones CSV cargadas.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante la carga en CsvLoader.");
                return OperationResult.Failure($"Error en CsvLoader: {ex.Message}");
            }
        }

        // ==========================================================
        // MÉTODOS PARA INSERTAR DIMENSIONES EN EL DATA WAREHOUSE
        // ==========================================================

        private void InsertarDimClientes(List<Clientes> clientes, SqlConnection connection, SqlTransaction transaction)
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
                cmd.Parameters.AddWithValue("@Nombre", (object)c.Nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)c.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", (object)c.Region ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertarDimProductos(List<Productos> productos, SqlConnection connection, SqlTransaction transaction)
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
                cmd.Parameters.AddWithValue("@Nombre", (object)p.Nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Categoria", (object)p.Categoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", p.Precio);

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertarDimFechasDesdeVentas(List<Ventas> ventas, SqlConnection connection, SqlTransaction transaction)
        {
            var cultura = new CultureInfo("es-DO");

            var fechas = ventas
                .Select(v => v.Fecha.Date)
                .Distinct()
                .ToList();

            foreach (var fecha in fechas)
            {
                int fechaKey = int.Parse(fecha.ToString("yyyyMMdd"));
                string nombreMes = cultura.DateTimeFormat.GetMonthName(fecha.Month);
                int trimestre = ((fecha.Month - 1) / 3) + 1;

                using var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM Dimension.DimFecha WHERE FechaKey = @FechaKey)
                BEGIN
                    INSERT INTO Dimension.DimFecha 
                    (FechaKey, Fecha, Dia, Mes, NombreMes, Anio, Trimestre)
                    VALUES 
                    (@FechaKey, @Fecha, @Dia, @Mes, @NombreMes, @Anio, @Trimestre);
                END;", connection, transaction);

                cmd.Parameters.AddWithValue("@FechaKey", fechaKey);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@Dia", fecha.Day);
                cmd.Parameters.AddWithValue("@Mes", fecha.Month);
                cmd.Parameters.AddWithValue("@NombreMes", nombreMes);
                cmd.Parameters.AddWithValue("@Anio", fecha.Year);
                cmd.Parameters.AddWithValue("@Trimestre", trimestre);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
