using Venalytix.Apication.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly EtlOrchestratorService _etl;

    public Worker(ILogger<Worker> logger, EtlOrchestratorService etl)
    {
        _logger = logger;
        _etl = etl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado.");

        // 🔥 EJECUTAR ETL
        _logger.LogInformation("🚀 Ejecutando proceso ETL...");

        var resultado = await _etl.EjecutarEtlCompletoAsync();

        if (resultado.IsSuccess)
            _logger.LogInformation($"✅ ETL completado: {resultado.Message}");
        else
            _logger.LogError($"❌ ETL falló: {resultado.Message}");

        // ❗ IMPORTANTE: Detener el Worker cuando termine el ETL
        _logger.LogInformation("⛔ Finalizando Worker luego del ETL.");
    }
}
