using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.Interfaces.IExtractor;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Extractors
{
    public class ApiExtractor : IExtractor
    {
        private readonly ILogger<ApiExtractor> _logger;
        private readonly HttpClient _httpClient;

        public ApiExtractor(ILogger<ApiExtractor> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando extracción desde API REST...");

                var response = await _httpClient.GetAsync("https://api.example.com/clientes");
                if (!response.IsSuccessStatusCode)
                    return OperationResult.Failure($"Error al consumir API: {response.StatusCode}");

                string json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Extracción desde API completada correctamente.");

                return OperationResult.Success(json, "Datos obtenidos desde la API.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error durante la extracción desde API.");
                return OperationResult.Failure($"Error en ApiExtractor: {ex.Message}");
            }
        }
    }
}
