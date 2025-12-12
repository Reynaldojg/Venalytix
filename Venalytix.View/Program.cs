using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Apication.Services;
using Venalytix.Apication.Services.Extractors;
using Venalytix.Apication.Services.Loaders;
using Venalytix.Apication.Services.Transformer;

var builder = Host.CreateDefaultBuilder(args)
.ConfigureServices((context, services) =>
{
    // EXTRACTORS
    services.AddHttpClient<ApiExtractor>();
    services.AddSingleton<IExtractor, ApiExtractor>();
    services.AddSingleton<IExtractor, CsvExtractor>();
    services.AddSingleton<IExtractor, DbExtractor>();

    // TRANSFORMERS
    services.AddSingleton<ITransformer, ApiTransformer>();
    services.AddSingleton<ITransformer, CsvTransformer>();
    services.AddSingleton<ITransformer, DbTransformer>();
    services.AddSingleton<ITransformer, FactVentasTransformer>();

    // LOADERS
    services.AddSingleton<ILoader, CsvLoader>();
    services.AddSingleton<ILoader, FactVentasLoader>();

    // ORCHESTRATOR
    services.AddSingleton<EtlOrchestratorService>();
    services.AddHostedService<Worker>();
});

await builder.RunConsoleAsync();

