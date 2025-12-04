using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Extractors
{
    public class MultiExtractor : IMultiExtractor
    {
        private readonly IEnumerable<IExtractor> _extractors;
        private readonly ILogger<MultiExtractor> _logger;

        public MultiExtractor(IEnumerable<IExtractor> extractors,
                              ILogger<MultiExtractor> logger)
        {
            _extractors = extractors;
            _logger = logger;
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            _logger.LogInformation("🔵 Iniciando MultiExtractor…");

            var resultados = new List<object>();

            foreach (var extractor in _extractors)
            {
                _logger.LogInformation("➡ Ejecutando extractor: {Extractor}",
                    extractor.GetType().Name);

                var resultado = await extractor.ExtraerAsync();

                if (!resultado.IsSuccess)
                {
                    _logger.LogWarning("⚠ Error en {Extractor}: {Mensaje}",
                        extractor.GetType().Name, resultado.Message);
                    continue;
                }

                if (resultado.Data != null)
                    resultados.Add(resultado.Data);
            }

            _logger.LogInformation("🔵 MultiExtractor completado.");

            return OperationResult.Success(resultados,
                "Extracción múltiple completada.");
        }
    }
}

