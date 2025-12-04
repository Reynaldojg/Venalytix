using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services
{
    public class EtlOrchestratorService
    {
        private readonly IExtractor _apiExtractor;
        private readonly IExtractor _csvExtractor;
        private readonly ITransformer _transformer;
        private readonly ILoader _loader;
        private readonly ILogger<EtlOrchestratorService> _logger;

        public EtlOrchestratorService(
            IEnumerable<IExtractor> extractors,
            IEnumerable<ITransformer> transformers,
            IEnumerable<ILoader> loaders,
            ILogger<EtlOrchestratorService> logger)
        {
            _apiExtractor = extractors.FirstOrDefault(e => e.GetType().Name.Contains("Api"))!;
            _csvExtractor = extractors.FirstOrDefault(e => e.GetType().Name.Contains("Csv"))!;
            _transformer = transformers.First();
            _loader = loaders.First();
            _logger = logger;
        }

        public async Task<OperationResult> EjecutarEtlCompletoAsync()
        {
            _logger.LogInformation("🏁 Iniciando ETL (con fallback CSV)…");

            OperationResult extraccion;


            if (_apiExtractor != null)
            {
                _logger.LogInformation("🌐 Intentando extracción desde API…");

                extraccion = await _apiExtractor.ExtraerAsync();

                if (extraccion.IsSuccess)
                {
                    _logger.LogInformation("✅ Datos obtenidos desde la API.");
                    return await TransformarYCargarAsync(extraccion.Data);
                }

                _logger.LogWarning("⚠ API falló: {msg}. Se usará CSV.", extraccion.Message);
            }


            _logger.LogInformation("📁 Iniciando extracción desde CSV…");

            extraccion = await _csvExtractor.ExtraerAsync();

            if (!extraccion.IsSuccess)
            {
                _logger.LogError("❌ CSV también falló. No hay datos para procesar.");
                return OperationResult.Failure("No se pudo obtener datos ni de API ni de CSV.");
            }

            _logger.LogInformation("📦 Datos cargados desde CSV correctamente.");

            return await TransformarYCargarAsync(extraccion.Data);
        }

        private async Task<OperationResult> TransformarYCargarAsync(object data)
        {

            _logger.LogInformation("🔄 Transformando datos…");

            var transformacion = _transformer.Transform(data);

            if (!transformacion.IsSuccess)
            {
                _logger.LogError("❌ Error transformando datos: {msg}", transformacion.Message);
                return transformacion;
            }


            _logger.LogInformation("📥 Cargando datos en el DW…");

            var carga = _loader.Load(transformacion.Data);

            if (!carga.IsSuccess)
            {
                // Fix: Cast dynamic argument to string to avoid CS1973
                _logger.LogError("❌ Error cargando datos: {msg}", (string)carga.Message);
                return carga;
            }

            _logger.LogInformation("🎉 ETL COMPLETADO EXITOSAMENTE.");

            return OperationResult.Success("ETL finalizado correctamente.");
        }
    }
}
