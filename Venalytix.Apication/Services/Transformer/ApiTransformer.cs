using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.CSV;
using Venalytix.Domain.OperationBase;

public class ApiTransformer : ITransformer
{
    private readonly ILogger<ApiTransformer> _logger;

    public ApiTransformer(ILogger<ApiTransformer> logger)
    {
        _logger = logger;
    }

    public OperationResult Transform(object data)
    {
        try
        {
            _logger.LogInformation("🔄 Iniciando transformación de datos provenientes de API…");

            // VALIDAR TIPO
            if (data is not List<Clientes> listaApi)
                return OperationResult.Failure("El ApiTransformer recibió un tipo no válido.");

            var listaTransformada = new List<Clientes>();

            foreach (var c in listaApi)
            {
                var cliente = new Clientes
                {
                    IdCLiente = c.IdCLiente,
                    Nombre = c.Nombre?.Trim(),
                    Email = c.Email?.Trim(),
                    Region = c.Region?.Trim()
                };

                listaTransformada.Add(cliente);
            }

            _logger.LogInformation("✅ Transformación de API completada. {count} registros procesados.", listaTransformada.Count);

            return OperationResult.Success(listaTransformada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error en ApiTransformer.");
            return OperationResult.Failure($"Error en ApiTransformer: {ex.Message}");
        }
    }
}
