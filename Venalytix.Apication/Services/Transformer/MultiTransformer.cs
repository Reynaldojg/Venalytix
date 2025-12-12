using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
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
            _logger.LogInformation("🟣 Iniciando MultiTransformer…");

            if (data == null)
                return OperationResult.Failure("El paquete recibido es nulo.");

            // Convertimos el paquete dinámico en tipado
            dynamic paquete = data;

            List<Clientes> clientes = paquete.Clientes;
            List<Productos> productos = paquete.Productos;
            List<Ventas> ventas = paquete.Ventas;

            var clientesTransformados = clientes;
            var productosTransformados = productos;
            var ventasTransformadas = ventas;

            // Ejecutar cada transformer según su tipo
            foreach (var transformer in _transformers)
            {
                _logger.LogInformation("➡ Ejecutando transformer: {T}", transformer.GetType().Name);

                if (transformer is CsvTransformer)
                {
                    var res = transformer.Transform(paquete);
                    if (res.IsSuccess) paquete = res.Data;
                }

                if (transformer is ApiTransformer)
                {
                    var res = transformer.Transform(paquete);
                    if (res.IsSuccess) paquete = res.Data;
                }
            }

            // Volvemos a armar el paquete final
            var paqueteFinal = new
            {
                Clientes = paquete.Clientes,
                Productos = paquete.Productos,
                Ventas = paquete.Ventas
            };

            _logger.LogInformation("🟣 MultiTransformer completado.");

            return OperationResult.Success(paqueteFinal);
        }
    }
}
