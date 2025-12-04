using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Transformer
{
    /// Transformer para los datos provenientes de CSV.
    /// En esta versión los datos ya vienen normalizados desde el extractor,
    /// así que solo validamos y los pasamos al Loader.
    public class CsvTransformer : ITransformer
    {
        private readonly ILogger<CsvTransformer> _logger;

        public CsvTransformer(ILogger<CsvTransformer> logger)
        {
            _logger = logger;
        }

        public OperationResult Transform(object data)
        {
            _logger.LogInformation(" Iniciando fase de transformación (CSV)...");

            if (data is null)
            {
                _logger.LogWarning(" El extractor no devolvió datos válidos (data = null).");
                return OperationResult.Failure("El extractor no devolvió datos válidos.");
            }

            // Aquí podrías agregar reglas extra si el profe lo pide.
            _logger.LogInformation(" Transformación CSV completada (sin cambios estructurales).");

            return OperationResult.Success(data, "Transformación CSV completada.");
        }
    }
}

