using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;
using Venalytix.Apication.Services.Mappers;
using Venalytix.Domain.OperationBase;
using Venalytix.Apication.Models.ModelsCSV;
using Venalytix.Apication.Interfaces.ETL;

namespace Venalytix.Apication.Services.Extractors
{
    public class CsvExtractor : IExtractor
    {
        private readonly ILogger<CsvExtractor> _logger;
        private readonly IConfiguration _config;

        public CsvExtractor(ILogger<CsvExtractor> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<OperationResult> ExtraerAsync()
        {
            try
            {
                _logger.LogInformation(" Iniciando extracción de archivos CSV externos...");

                
                var basePath = _config["CsvSettings:BasePath"];
                var outputPath = _config["CsvSettings:OutputPath"];

                if (string.IsNullOrEmpty(basePath) || !Directory.Exists(basePath))
                    return OperationResult.Failure($"❌ No se encontró la carpeta de origen: {basePath}");

                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);

               
                var customers = await LeerCsvAsync<CustomerCsv>(Path.Combine(basePath, "customers.csv"));
                var products = await LeerCsvAsync<ProductCsv>(Path.Combine(basePath, "products.csv"));
                var orders = await LeerCsvAsync<OrderCsv>(Path.Combine(basePath, "orders.csv"));
                var details = await LeerCsvAsync<OrderDetailCsv>(Path.Combine(basePath, "order_details.csv"));

                if (!customers.Any() || !products.Any() || !orders.Any() || !details.Any())
                    return OperationResult.Failure("❌ Uno o más archivos CSV están vacíos o no se pudieron leer.");

                
                var clientes = CsvToDomainMapper.MapClientes(customers);
                var productos = CsvToDomainMapper.MapProductos(products);
                var ventas = CsvToDomainMapper.MapVentas(orders, details);

                
                var paquete = new
                {
                    Clientes = clientes,
                    Productos = productos,
                    Ventas = ventas
                };

                
                var jsonPath = Path.Combine(outputPath, "resultado_extraccion.json");

                await File.WriteAllTextAsync(
                    jsonPath,
                    JsonSerializer.Serialize(paquete, new JsonSerializerOptions { WriteIndented = true })
                );

                _logger.LogInformation(" Extracción completada. JSON generado en {0}", jsonPath);

                return OperationResult.Success(paquete, "Extracción CSV completada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError (ex, " Error durante la extracción de CSV externos");
                return OperationResult.Failure($"Error en CsvExtractor: {ex.Message}");
            }
        }

        private static async Task<List<T>> LeerCsvAsync<T>(string ruta)
        {
            if (!File.Exists(ruta))
                return new List<T>();

            using var reader = new StreamReader(ruta);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<T>().ToList();
            await Task.Yield(); 
            return records;
        }
    }
}

