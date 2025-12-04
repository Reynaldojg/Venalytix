//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Venalytix.Apication.Services;
//using Venalytix.Apication.Services.Extractors;
//using System;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Venalytix.Apication.Interfaces.ETL;

//namespace Venalytix.Apication
//{
//    public class Prueba
//    {
//        public static async Task Main()
//        {
//            Console.WriteLine(" Iniciando prueba de extracción ETL...\n");

//            // 1️⃣ Logger
//            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
//            var logger = loggerFactory.CreateLogger<EtlOrchestratorService>();

//            // 2️⃣ Leer configuración (appsettings.json debe estar en este proyecto)
//            var config = new ConfigurationBuilder()
//                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                .Build();

//            // 3️⃣ Saber qué extractor utilizar (CSV o API)
//            string tipoExtractor = config["ExtractorSettings:Tipo"]?.ToUpper() ?? "CSV";

//            IExtractor extractor;

//            if (tipoExtractor == "API")
//            {
//                // Crear extractor API
//                var apiLogger = loggerFactory.CreateLogger<ApiExtractor>();
//                var httpClient = new HttpClient
//                {
//                    BaseAddress = new Uri(config["ApiSettings:BaseUrl"])
//                };

//                extractor = new ApiExtractor(apiLogger, httpClient, config);
//                Console.WriteLine(" Usando APIExtractor...\n");
//            }
//            else
//            {
//                // Crear extractor CSV
//                var csvLogger = loggerFactory.CreateLogger<CsvExtractor>();
//                extractor = new CsvExtractor(csvLogger, config);
//                Console.WriteLine(" Usando CsvExtractor...\n");
//            }

//            // 4️⃣ Crear Orquestador
//            var orchestrator = new EtlOrchestratorService(extractor, logger);

//            // 5️⃣ Ejecutar proceso
//            var resultado = await orchestrator.EjecutarEtlCompletoAsync();

//            // 6️⃣ Mostrar resultado
//            Console.WriteLine("\n===============================");
//            Console.WriteLine($" Mensaje: {resultado.Message}");
//            Console.WriteLine($" Éxito: {resultado.IsSuccess}");
//            Console.WriteLine("===============================\n");

//            if (resultado.IsSuccess)
//            {
//                Console.WriteLine(" Proceso completado correctamente.");
//            }
//            else
//            {
//                Console.WriteLine("Error: " + resultado.Message);
//            }

//            Console.WriteLine("\nPresiona una tecla para salir...");
//            Console.ReadKey();
//        }
//    }
//}
