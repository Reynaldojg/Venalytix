using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;
using Microsoft.Extensions.Logging;

namespace Venalytix.Apication.Services.Loaders
{
    public class MultiLoader : IMultiLoader
    {
        private readonly IEnumerable<ILoader> _loaders;
        private readonly ILogger<MultiLoader> _logger;

        public MultiLoader(IEnumerable<ILoader> loaders,
                           ILogger<MultiLoader> logger)
        {
            _loaders = loaders;
            _logger = logger;
        }

        public OperationResult Load(object data)
        {
            var paquetes = data as IEnumerable<object>;
            var resultados = new List<object>();

            int index = 0;
            foreach (var loader in _loaders)
            {
                if (index >= paquetes.Count()) break;

                var paquete = paquetes.ElementAt(index);

                _logger.LogInformation("🔵 Ejecutando loader: {L}", loader.GetType().Name);

                var r = loader.Load(paquete);

                if (!r.IsSuccess)
                {
                    _logger.LogWarning("⚠ Error en loader {L}: {M}",
                        loader.GetType().Name, r.Message);
                }
                else
                {
                    resultados.Add(r.Data);
                }

                index++;
            }

            return OperationResult.Success(resultados, "Cargas ejecutadas.");
        }
    }
}
