using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Extractors
{
    public class DatabaseExtractor : IExtractor
    {
        private readonly ILogger<DatabaseExtractor> _logger;
        private readonly string _connectionString;

        public DatabaseExtractor(ILogger<DatabaseExtractor> logger)
        {
            _logger = logger;
            _connectionString = "Server=.;Database=VentasHistoricas;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando extracción desde base de datos externa...");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT COUNT(*) FROM Ventas", connection);
                    int total = (int)await command.ExecuteScalarAsync();

                    return OperationResult.Success(total, $"Extracción desde base externa completada ({total} registros).");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error durante la extracción desde la base de datos.");
                return OperationResult.Failure($"Error en DatabaseExtractor: {ex.Message}");
            }
        }
    }
}
