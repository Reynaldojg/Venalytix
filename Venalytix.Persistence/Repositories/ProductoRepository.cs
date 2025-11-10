using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.DTOS.ProductosDTO;
using Venalytix.Apication.Interfaces.IRepositories;
using Venalytix.Domain.OperationBase;
using Venalytix.Persistence.Base;
using System.Data;

namespace Venalytix.Persistence.Repositories
{
    public class ProductoRepository : BaseRepository<ProductoRepository>, IProductoRepository
    {
        public ProductoRepository(string connectionString, ILogger<ProductoRepository> logger)
            : base(connectionString, logger) { }

        public async Task<OperationResult> AgregarAsync(SaveProductoDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                return OperationResult.Failure("Datos del producto inválidos.");

            return await ExecuteNonQueryAsync("sp_AgregarProducto", cmd =>
            {
                cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
                cmd.Parameters.AddWithValue("@Categoria", dto.Categoria ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", dto.Precio);
            });
        }

        public async Task<OperationResult> ActualizarAsync(UpdateProductoDTO dto)
        {
            if (dto == null || dto.IdProducto <= 0)
                return OperationResult.Failure("Producto no válido.");

            return await ExecuteNonQueryAsync("sp_ActualizarProducto", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdProducto", dto.IdProducto);
                cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
                cmd.Parameters.AddWithValue("@Categoria", dto.Categoria);
                cmd.Parameters.AddWithValue("@Precio", dto.Precio);
            });
        }

        public async Task<OperationResult> EliminarAsync(int id)
        {
            if (id <= 0)
                return OperationResult.Failure("ID no válido.");

            return await ExecuteNonQueryAsync("sp_EliminarProducto", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdProducto", id);
            });
        }

        public async Task<OperationResult> ObtenerPorAsync(int id)
        {
            return await ExecuteReaderAsync("sp_ObtenerProductoPorId", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdProducto", id);
            },
            reader => new ObtenerProductoDTO
            {
                IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio"))
            });
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await ExecuteReaderAsync("sp_ObtenerTodosProductos", cmd => { },
            reader => new ObtenerProductoDTO
            {
                IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio"))
            });
        }
    }
}
