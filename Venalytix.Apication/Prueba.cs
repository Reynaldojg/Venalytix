using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.Services;
using Venalytix.Apication.Services.Extractors;
using Venalytix.Apication.Interfaces.IExtractor;
using System;
using System.Threading.Tasks;

namespace Venalytix.Apication
{
    public class Prueba
    {
        public static async Task Main()
        {
            Console.WriteLine("🚀 Iniciando prueba de extracción ETL...\n");

            // 1️⃣ Crear el logger para consola
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<EtlOrchestratorService>();
            var csvLogger = loggerFactory.CreateLogger<CsvExtractor>();

            // 2️⃣ Leer configuración desde appsettings.json
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 3️⃣ Instanciar el extractor CSV y el orquestador
            IExtractor extractor = new CsvExtractor(csvLogger, config);
            var etlOrchestrator = new EtlOrchestratorService(extractor, logger);

            // 4️⃣ Ejecutar el proceso ETL
            var resultado = await etlOrchestrator.EjecutarEtlCompletoAsync();

            // 5️⃣ Mostrar el resultado
            Console.WriteLine("\n===============================");
            Console.WriteLine($"🧩 Mensaje: {resultado.Message}");
            Console.WriteLine($"📊 Éxito: {resultado.IsSuccess}");
            Console.WriteLine("===============================\n");

            if (resultado.IsSuccess)
            {
                Console.WriteLine("✅ Proceso completado correctamente.");
                Console.WriteLine("📁 Revisa el archivo JSON generado en la carpeta de salida.");
            }
            else
            {
                Console.WriteLine("❌ Error: " + resultado.Message);
            }

            Console.WriteLine("\nPresiona una tecla para salir...");
            Console.ReadKey();
        }
    }
}
