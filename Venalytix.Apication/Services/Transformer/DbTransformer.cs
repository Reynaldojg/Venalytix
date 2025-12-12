using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Domain.CSV;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Transformer
{
    public class DbTransformer : ITransformer
    {
        private readonly ILogger<DbTransformer> _logger;

        public DbTransformer(ILogger<DbTransformer> logger)
        {
            _logger = logger;
        }

        public OperationResult Transform(object data)
        {
            _logger.LogInformation("🔄 Transformando datos de BDD externa...");

            if (data is not List<Ventas> ventas)
                return OperationResult.Failure("Datos inválidos para DbTransformer.");

            foreach (var v in ventas)
            {
                if (v.Cantidad <= 0) v.Cantidad = 1;
                if (v.Precio < 0) v.Precio = 0;
            }

            return OperationResult.Success(ventas, "Transformación BDD externa completada.");
        }
    }
}

