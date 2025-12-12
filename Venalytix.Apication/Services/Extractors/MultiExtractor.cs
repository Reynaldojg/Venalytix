using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
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

            // Listas para consolidar lo que devuelvan los extractores
            var listaClientes = new List<Clientes>();
            var listaProductos = new List<Productos>();
            var listaVentas = new List<Ventas>();

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

                if (resultado.Data == null)
                    continue;

                // --- Clasificación de resultados por tipo ---

                if (resultado.Data is List<Clientes> clientes)
                    listaClientes.AddRange(clientes);

                else if (resultado.Data is List<Productos> productos)
                    listaProductos.AddRange(productos);

                else if (resultado.Data is List<Ventas> ventas)
                    listaVentas.AddRange(ventas);

                else
                    _logger.LogWarning("⚠ Tipo desconocido recibido en MultiExtractor: {Tipo}",
                        (object)resultado.Data.GetType().Name);
            }

            // Crear paquete consolidado
            var paquete = new
            {
                Clientes = listaClientes,
                Productos = listaProductos,
                Ventas = listaVentas
            };

            _logger.LogInformation("🔵 MultiExtractor completado.");

            return OperationResult.Success(paquete,
                "Extracción múltiple completada correctamente.");
        }
    }
}
