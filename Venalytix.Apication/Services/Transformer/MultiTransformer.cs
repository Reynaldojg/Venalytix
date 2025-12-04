using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Transformer
{
    public class MultiTransformer : ITransformer
    {
        private readonly IEnumerable<ITransformer> _transformers;
        private readonly ILogger<MultiTransformer> _logger;

        public MultiTransformer(IEnumerable<ITransformer> transformers,
                                ILogger<MultiTransformer> logger)
        {
            _transformers = transformers;
            _logger = logger;
        }

        public OperationResult Transform(object data)
        {
            _logger.LogInformation(" Iniciando MultiTransformer…");

            var listaDatos = data as IEnumerable<object>;
            var resultadoFinal = new List<object>();

            int i = 0;
            foreach (var transformer in _transformers)
            {
                if (i >= listaDatos.Count()) break; // proteger índice

                var datos = listaDatos.ElementAt(i);

                var result = transformer.Transform(datos);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("⚠️ Error transformando con {T}: {M}",
                        transformer.GetType().Name,
                        result.Message);
                }
                else if (result.Data != null)
                {
                    resultadoFinal.Add(result.Data);
                }

                i++;
            }

            _logger.LogInformation("🟣 MultiTransformer completado.");

            return OperationResult.Success(resultadoFinal);
        }
    }
}
