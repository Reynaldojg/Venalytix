using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Apication.Services;
using Venalytix.Apication.Services.Extractors;
using Venalytix.Apication.Services.Loaders;
using Venalytix.Apication.Services.Transformer;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        string tipo = config["ExtractorSettings:Tipo"]?.ToUpper() ?? "CSV";


        services.AddHttpClient<ApiExtractor>();
        services.AddSingleton<IExtractor, ApiExtractor>();
        services.AddSingleton<IExtractor, CsvExtractor>();
        Console.WriteLine(" Registrados: ApiExtractor + CsvExtractor");

        services.AddSingleton<ITransformer, CsvTransformer>();
        Console.WriteLine(" Registrado: CsvTransformer");


        services.AddSingleton<ILoader, CsvLoader>();
        Console.WriteLine(" Registrado: CsvLoader");


        services.AddSingleton<EtlOrchestratorService>();

        services.AddHostedService<Worker>();
    });

await builder.RunConsoleAsync();

