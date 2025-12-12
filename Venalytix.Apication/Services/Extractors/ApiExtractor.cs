using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
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

            // URL base desde appsettings.json
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation("🌐 Iniciando extracción desde API REST...");

                string endpoint = _config["ApiSettings:ClientesEndpoint"];

                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                    return OperationResult.Failure($"❌ Error al consumir API: {response.StatusCode}");

                string json = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📥 JSON recibido: {json}", json);

                // Intentar detectar si viene un objeto con "data"
                using var doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                List<Clientes>? listaClientes;

                if (root.ValueKind == JsonValueKind.Object &&
                    root.TryGetProperty("data", out JsonElement dataNode))
                {
                    // API devuelve un objeto con "data"
                    listaClientes = JsonSerializer.Deserialize<List<Clientes>>(dataNode.GetRawText());
                }
                else
                {
                    // API devuelve una lista directa
                    listaClientes = JsonSerializer.Deserialize<List<Clientes>>(json);
                }

                if (listaClientes == null)
                    return OperationResult.Failure("❌ No se pudieron deserializar los datos de la API.");

                _logger.LogInformation("✅ Extracción desde API completada correctamente.");

                return OperationResult.Success(listaClientes, "Clientes obtenidos desde API.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante la extracción desde API.");
                return OperationResult.Failure($"Error en ApiExtractor: {ex.Message}");
            }
        }
    }
}

