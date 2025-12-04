using System.Text.Json;
using Venalytix.Apication.Interfaces.ELT;
using Venalytix.Apication.Interfaces.ETL;
using Venalytix.Domain.OperationBase;

public class ApiTransformer : ITransformer
{
    public OperationResult Transform(object data)
    {
        try
        {
            string json = data.ToString();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, options);

            foreach (var item in list)
            {
                foreach (var key in item.Keys.ToList())
                {
                    if (item[key] is string text)
                    {
                        item[key] = text.Trim();
                    }
                }
            }

            return OperationResult.Success(list, "Transformación de API completada.");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Error en ApiTransformer: {ex.Message}");
        }
    }
}
