using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.IExtractor;
using Venalytix.Domain.OperationBase;
using System;
using System.Threading.Tasks;

namespace Venalytix.Apication.Services
{
    /// Servicio orquestador del proceso ETL: controla la secuencia de Extracción, Transformación y Carga.
    public class EtlOrchestratorService
    {
        private readonly IExtractor ExtractoAll;
        private readonly ILogger<EtlOrchestratorService> _logger;

        public EtlOrchestratorService(IExtractor extractoAll, ILogger<EtlOrchestratorService> logger)
        {
            ExtractoAll = extractoAll;
            _logger = logger;
        }

        /// Ejecuta solo la fase de extracción (lectura y guardado de datos CSV).
        public async Task<OperationResult> EjecutarExtraccionAsync()
        {
            _logger.LogInformation("🚀 Iniciando fase de extracción del proceso ETL...");

            try
            {
                var resultadoExtraccion = await ExtractoAll.ExtraerAsync(); 

                if (!resultadoExtraccion.IsSuccess)
                {
                    _logger.LogWarning("⚠️ Error en la extracción: {Mensaje}", resultadoExtraccion.Message);
                    return OperationResult.Failure("Error en la extracción: " + resultadoExtraccion.Message);
                }

                _logger.LogInformation("✅ Extracción completada correctamente.");
                return OperationResult.Success(resultadoExtraccion.Data, "Extracción ETL completada con éxito.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado durante la ejecución de la extracción.");
                return OperationResult.Failure($"Error inesperado: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation("🔚 Fase de extracción finalizada.");
            }
        }

        /// Ejecuta el proceso ETL completo (por ahora solo la fase de extracción activa).
        public async Task<OperationResult> EjecutarEtlCompletoAsync()
        {
            _logger.LogInformation("🏁 Iniciando proceso ETL completo...");

            var extraccion = await EjecutarExtraccionAsync();

            if (!extraccion.IsSuccess)
            {
                _logger.LogWarning("❌ Proceso ETL detenido. Error en la fase de extracción.");
                return extraccion;
            }

            // 🔜 (Lugar reservado para futuras fases)
            // var transformacion = await EjecutarTransformacionAsync(extraccion.Data);
            // var carga = await EjecutarCargaAsync(transformacion.Data);

            _logger.LogInformation("📦 Proceso ETL finalizado correctamente (fase de extracción completada).");
            return OperationResult.Success(extraccion.Data, "Proceso ETL completado exitosamente (solo extracción).");
        }
    }
}


