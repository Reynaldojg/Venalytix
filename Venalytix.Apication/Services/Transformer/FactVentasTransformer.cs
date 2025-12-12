using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Domain.CSV;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Transformer
{
    public class FactVentasTransformer : ITransformer
    {
        private readonly ILogger<FactVentasTransformer> _logger;

        public FactVentasTransformer(ILogger<FactVentasTransformer> logger)
        {
            _logger = logger;
        }

        public OperationResult Transform(object data)
        {
            try
            {
                _logger.LogInformation(" Transformando datos para FactVentas...");

                List<Ventas> listaVentas = null;

                // Caso 1: El data ES directamente la lista de ventas
                if (data is List<Ventas> ventasDirectas)
                {
                    listaVentas = ventasDirectas;
                }
                // Caso 2: data viene como paquete anónimo { Clientes, Productos, Ventas }
                else
                {
                    try
                    {
                        dynamic paquete = data;
                        listaVentas = paquete.Ventas as List<Ventas>;
                    }
                    catch
                    {
                        listaVentas = null;
                    }
                }

                if (listaVentas == null)
                    return OperationResult.Failure("❌ No se encontraron ventas en el paquete recibido.");

                // Reglas mínimas de validación
                foreach (var v in listaVentas)
                {
                    if (v.Cantidad <= 0)
                        v.Cantidad = 1;

                    if (v.Precio < 0)
                        v.Precio = 0;
                }

                _logger.LogInformation("✅ Ventas transformadas correctamente: {0} registros.", listaVentas.Count);

                return OperationResult.Success(listaVentas, "Ventas transformadas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante transformación de ventas.");
                return OperationResult.Failure($"Error en FactVentasTransformer: {ex.Message}");
            }
        }
    }
}
