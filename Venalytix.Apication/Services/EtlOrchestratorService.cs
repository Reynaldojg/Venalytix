using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services
{
    public class EtlOrchestratorService
    {
        private readonly IExtractor _apiExtractor;
        private readonly IExtractor _csvExtractor;
        private readonly IExtractor _dbExtractor;

        private readonly ITransformer _apiTransformer;
        private readonly ITransformer _csvTransformer;
        private readonly ITransformer _dbTransformer;
        private readonly ITransformer _factVentasTransformer;

        private readonly ILoader _dimLoader;
        private readonly ILoader _factVentasLoader;

        private readonly ILogger<EtlOrchestratorService> _logger;

        public EtlOrchestratorService(
            IEnumerable<IExtractor> extractors,
            IEnumerable<ITransformer> transformers,
            IEnumerable<ILoader> loaders,
            ILogger<EtlOrchestratorService> logger)
        {
            _apiExtractor = extractors.First(e => e.GetType().Name.Contains("Api"));
            _csvExtractor = extractors.First(e => e.GetType().Name.Contains("Csv"));
            _dbExtractor = extractors.First(e => e.GetType().Name.Contains("Db"));

            _apiTransformer = transformers.First(t => t.GetType().Name.Contains("Api"));
            _csvTransformer = transformers.First(t => t.GetType().Name.Contains("Csv"));
            _dbTransformer = transformers.First(t => t.GetType().Name.Contains("Db"));
            _factVentasTransformer = transformers.First(t => t.GetType().Name.Contains("FactVentas"));

            _dimLoader = loaders.First(l => l.GetType().Name.Contains("CsvLoader"));
            _factVentasLoader = loaders.First(l => l.GetType().Name.Contains("FactVentas"));

            _logger = logger;
        }

        public async Task<OperationResult> EjecutarEtlCompletoAsync()
        {
            _logger.LogInformation("🏁 Iniciando ETL MULTIFUENTE...");

            var fuentes = new[]
            {
                (_apiExtractor, _apiTransformer, "API"),
                (_dbExtractor,  _dbTransformer,  "BDD Externa"),
                (_csvExtractor, _csvTransformer, "CSV")
            };

            foreach (var (extractor, transformer, nombre) in fuentes)
            {
                _logger.LogInformation($"🔍 Probando fuente: {nombre}");

                var extraccion = await extractor.ExtraerAsync();

                if (!extraccion.IsSuccess) continue;

                var transformacion = transformer.Transform(extraccion.Data);
                if (!transformacion.IsSuccess) continue;

                var cargaDims = _dimLoader.Load(transformacion.Data);
                if (!cargaDims.IsSuccess) continue;

                var factTransform = _factVentasTransformer.Transform(transformacion.Data);
                if (!factTransform.IsSuccess) continue;

                var cargaFact = _factVentasLoader.Load(factTransform.Data);
                if (!cargaFact.IsSuccess) continue;

                _logger.LogInformation($"✅ ETL completado usando {nombre}");
                return OperationResult.Success("ETL completado correctamente.");
            }

            return OperationResult.Failure("❌ Ninguna fuente pudo completar el ETL.");
        }
    }
}
