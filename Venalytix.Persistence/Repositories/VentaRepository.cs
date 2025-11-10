using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.DTOS.VentasDTO;
using Venalytix.Apication.Interfaces.IRepositories;
using Venalytix.Domain.OperationBase;
using Venalytix.Persistence.Base;
using System.Data;

namespace Venalytix.Persistence.Repositories
{
    public class VentaRepository : BaseRepository<VentaRepository>, IVentasRepository
    {
        public VentaRepository(string connectionString, ILogger<VentaRepository> logger)
            : base(connectionString, logger) { }

        public async Task<OperationResult> AgregarAsync(SaveVentaDTO dto)
        {
            if (dto == null || dto.IdCliente <= 0 || dto.IdProducto <= 0)
                return OperationResult.Failure("Datos de la venta inválidos.");

            return await ExecuteNonQueryAsync("sp_AgregarVenta", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdCliente", dto.IdCliente);
                cmd.Parameters.AddWithValue("@IdProducto", dto.IdProducto);
                cmd.Parameters.AddWithValue("@Cantidad", dto.Cantidad);
                cmd.Parameters.AddWithValue("@Precio", dto.Precio);
            });
        }

        public async Task<OperationResult> ActualizarAsync(UpdateVentaDTO dto)
        {
            if (dto == null)
                return OperationResult.Failure("Datos inválidos.");

            return await ExecuteNonQueryAsync("sp_ActualizarVenta", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdCliente", dto.IdCliente);
                cmd.Parameters.AddWithValue("@IdProducto", dto.IdProducto);
                cmd.Parameters.AddWithValue("@Cantidad", dto.Cantidad);
                cmd.Parameters.AddWithValue("@Precio", dto.Precio);
            });
        }

        public async Task<OperationResult> EliminarAsync(int id)
        {
            if (id <= 0)
                return OperationResult.Failure("ID no válido.");

            return await ExecuteNonQueryAsync("sp_EliminarVenta", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdVenta", id);
            });
        }

        public async Task<OperationResult> ObtenerPorAsync(int id)
        {
            return await ExecuteReaderAsync("sp_ObtenerVentaPorId", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdVenta", id);
            },
            reader => new ObtenerVentaDTO
            {
                IdVenta = reader.GetInt32(reader.GetOrdinal("IdVenta")),
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio"))
            });
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await ExecuteReaderAsync("sp_ObtenerTodasLasVentas", cmd => { },
            reader => new ObtenerVentaDTO
            {
                IdVenta = reader.GetInt32(reader.GetOrdinal("IdVenta")),
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio"))
            });
        }
    }
}
