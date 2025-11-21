using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Venalytix.Apication.Interfaces.IExtractor;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Extractors
{
    public class ApiExtractor : IExtractor
    {
        private readonly ILogger<ApiExtractor> _logger;
        private readonly HttpClient _httpClient;

        public ApiExtractor(ILogger<ApiExtractor> logger, HttpClient httpClient, Microsoft.Extensions.Configuration.IConfigurationRoot config)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation("🌐 Iniciando extracción desde API REST...");

                // 👉 Endpoint de tu propia API
                var response = await _httpClient.GetAsync("https://localhost:7101/api/Clientes");

                if (!response.IsSuccessStatusCode)
                {
                    return OperationResult.Failure(
                        $"❌ Error al consumir API: {response.StatusCode}"
                    );
                }

                string json = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("✅ Extracción desde API completada correctamente.");

                return OperationResult.Success(json, "Datos obtenidos desde la API.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante la extracción desde API.");
                return OperationResult.Failure($"Error en ApiExtractor: {ex.Message}");
            }
        }
    }
}

