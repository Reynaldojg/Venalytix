using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Services.Loaders
{
    public class ApiLoader : ILoader
    {
        private readonly ILogger<ApiLoader> _logger;
        private readonly string _connectionString;

        public ApiLoader(ILogger<ApiLoader> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("No se encontró 'DefaultConnection' en appsettings.json");
        }

        public OperationResult Load(object data)
        {
            try
            {
                _logger.LogInformation(" Iniciando carga hacia VentasOperacional (API Loader)...");

                var registros = data as List<Dictionary<string, object>>;
                if (registros == null || !registros.Any())
                    return OperationResult.Failure(" No hay datos para cargar.");

                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                foreach (var fila in registros)
                {
                    string tipoDato = DetectarTipo(fila);

                    if (tipoDato == "Cliente")
                        CargarCliente(fila, connection);
                    else if (tipoDato == "Producto")
                        CargarProducto(fila, connection);
                }

                _logger.LogInformation(" Carga completada correctamente.");
                return OperationResult.Success(null, "Carga API completada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante la carga en ApiLoader.");
                return OperationResult.Failure($"Error en ApiLoader: {ex.Message}");
            }
        }

        // Detectamos si vienen clientes o productos
        private string DetectarTipo(Dictionary<string, object> row)
        {
            if (row.ContainsKey("Region")) return "Cliente";  // Los clientes tienen región
            if (row.ContainsKey("Categoria")) return "Producto"; // Los productos tienen categoría
            return "Desconocido";
        }

        // Cargar Cliente
        private void CargarCliente(Dictionary<string, object> row, SqlConnection connection)
        {
            using var cmd = new SqlCommand("sp_InsertarCliente", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", row["Nombre"]);
            cmd.Parameters.AddWithValue("@Email", row["Email"]);
            cmd.Parameters.AddWithValue("@Region", row["Region"]);
            cmd.Parameters.AddWithValue("@FechaRegistro", row["FechaRegistro"]);

            cmd.ExecuteNonQuery();
        }

        // Cargar Producto
        private void CargarProducto(Dictionary<string, object> row, SqlConnection connection)
        {
            using var cmd = new SqlCommand("sp_InsertarProducto", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", row["Nombre"]);
            cmd.Parameters.AddWithValue("@Categoria", row["Categoria"]);
            cmd.Parameters.AddWithValue("@Precio", row["Precio"]);
            cmd.Parameters.AddWithValue("@Stock", row["Stock"]);
            cmd.Parameters.AddWithValue("@Activo", row["Activo"]);

            cmd.ExecuteNonQuery();
        }
    }
}