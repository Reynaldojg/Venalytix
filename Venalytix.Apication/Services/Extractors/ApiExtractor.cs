using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Extractors
{
    public class ApiExtractor : IExtractor
    {
        private readonly ILogger<ApiExtractor> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ApiExtractor(
            ILogger<ApiExtractor> logger,
            HttpClient httpClient,
            IConfiguration config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config;

            // 🟢 Configurar automáticamente la URL base desde appsettings
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation("🌐 Iniciando extracción desde API REST...");

                // 👉 Endpoint definido en appsettings.json
                string endpoint = _config["ApiSettings:ClientesEndpoint"];

                var response = await _httpClient.GetAsync(endpoint);

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
