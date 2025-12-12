using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Extractors
{
    public class DbExtractor : IExtractor
    {
        private readonly ILogger<DbExtractor> _logger;
        private readonly IConfiguration _config;

        public DbExtractor(ILogger<DbExtractor> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation(" Extrayendo datos desde BDD externa...");

                var connStr = _config.GetConnectionString("ExternalDbConnection");
                var ventas = new List<Ventas>();

                using var conn = new SqlConnection(connStr);
                await conn.OpenAsync();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        IdVenta,
                        IdCliente,
                        IdProducto,
                        Cantidad,
                        Precio,
                        Fecha
                    FROM VentasExternas
                ", conn);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ventas.Add(new Ventas
                    {
                        IdVentas = reader.GetInt32(0),
                        IdCliente = reader.GetInt32(1),
                        IdProducto = reader.GetInt32(2),
                        Cantidad = reader.GetInt32(3),
                        Precio = reader.GetDecimal(4),
                        Fecha = reader.GetDateTime(5)
                    });
                }

                return OperationResult.Success(ventas, "Extracción desde BDD externa completada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en DbExtractor");
                return OperationResult.Failure($"Error DbExtractor: {ex.Message}");
            }
        }
    }
}
