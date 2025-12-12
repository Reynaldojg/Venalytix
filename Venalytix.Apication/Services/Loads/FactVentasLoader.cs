using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Loaders
{
    public class FactVentasLoader : ILoader
    {
        private readonly ILogger<FactVentasLoader> _logger;
        private readonly string _connectionString;

        public FactVentasLoader(ILogger<FactVentasLoader> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DWConnection")
                ?? throw new Exception("No se encontró 'DWConnection' en appsettings.json");
        }

        public OperationResult Load(object data)
        {
            try
            {
                _logger.LogInformation("📦 Iniciando carga a FACTVENTAS...");

                if (data is not List<Ventas> ventas)
                    return OperationResult.Failure("❌ FactVentasLoader recibió un tipo de datos no válido.");

                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                foreach (var v in ventas)
                {
                    int clienteKey = ObtenerClienteKey(v.IdCliente, connection, transaction);
                    int productoKey = ObtenerProductoKey(v.IdProducto, connection, transaction);
                    int fechaKey = Convert.ToInt32(v.Fecha.ToString("yyyyMMdd"));

                    if (clienteKey == 0 || productoKey == 0)
                    {
                        _logger.LogWarning("⚠ Venta omitida por claves inválidas. VentaID={0}", v.IdVentas);
                        continue;
                    }

                    using var cmd = new SqlCommand(@"
                        INSERT INTO Fact.FactVentas
                        (VentaID, ClienteKey, ProductoKey, Cantidad, Precio, Fecha, FechaKey)
                        VALUES
                        (@VentaID, @ClienteKey, @ProductoKey, @Cantidad, @Precio, @Fecha, @FechaKey);
                    ", connection, transaction);

                    cmd.Parameters.AddWithValue("@VentaID", v.IdVentas);
                    cmd.Parameters.AddWithValue("@ClienteKey", clienteKey);
                    cmd.Parameters.AddWithValue("@ProductoKey", productoKey);
                    cmd.Parameters.AddWithValue("@Cantidad", v.Cantidad);
                    cmd.Parameters.AddWithValue("@Precio", v.Precio);
                    cmd.Parameters.AddWithValue("@Fecha", v.Fecha);
                    cmd.Parameters.AddWithValue("@FechaKey", fechaKey);

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                _logger.LogInformation("🎉 FACTVENTAS cargado correctamente.");

                return OperationResult.Success("FactVentas cargado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en FactVentasLoader.");
                return OperationResult.Failure($"Error en FactVentasLoader: {ex.Message}");
            }
        }

        private int ObtenerClienteKey(int idCliente, SqlConnection connection, SqlTransaction transaction)
        {
            using var cmd = new SqlCommand(@"
                SELECT ClienteKey FROM Dimension.DimClientes WHERE ClienteID = @IdCliente
            ", connection, transaction);

            cmd.Parameters.AddWithValue("@IdCliente", idCliente);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        private int ObtenerProductoKey(int idProducto, SqlConnection connection, SqlTransaction transaction)
        {
            using var cmd = new SqlCommand(@"
                SELECT ProductoKey FROM Dimension.DimProductos WHERE ProductoID = @IdProducto
            ", connection, transaction);

            cmd.Parameters.AddWithValue("@IdProducto", idProducto);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
