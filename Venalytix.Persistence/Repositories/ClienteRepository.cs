using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Venalytix.Apication.Interfaces.IRepositories;
using Venalytix.Domain.OperationBase;
using Venalytix.Persistence.Base;
using System.Data;
using Venalytix.Apication.DTOS.ClientesDTO;

namespace Venalytix.Persistence.Repositories
{
    public class ClienteRepository : BaseRepository<ClienteRepository>, IClienteRepository
    {
        public ClienteRepository(string connectionString, ILogger<ClienteRepository> logger)
            : base(connectionString, logger) { }

        public async Task<OperationResult> AgregarAsync(SaveClienteDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                return OperationResult.Failure("Datos inválidos del cliente.");

            return await ExecuteNonQueryAsync("sp_AgregarCliente", cmd =>
            {
                cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
                cmd.Parameters.AddWithValue("@Email", dto.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", dto.Region ?? (object)DBNull.Value);
            });
        }

        public async Task<OperationResult> ActualizarAsync(UpdateClienteDTO dto)
        {
            if (dto == null || dto.IdCliente <= 0)
                return OperationResult.Failure("Cliente no válido.");

            return await ExecuteNonQueryAsync("sp_ActualizarCliente", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdCliente", dto.IdCliente);
                cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@Region", dto.Region);
            });
        }

        public async Task<OperationResult> EliminarAsync(int id)
        {
            if (id <= 0)
                return OperationResult.Failure("ID no válido.");

            return await ExecuteNonQueryAsync("sp_EliminarCliente", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdCliente", id);
            });
        }

        public async Task<OperationResult> ObtenerPorAsync(int id)
        {
            return await ExecuteReaderAsync("sp_ObtenerClientePorId", cmd =>
            {
                cmd.Parameters.AddWithValue("@IdCliente", id);
            },
            reader => new ObtenerClienteDTO
            {
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Region = reader.GetString(reader.GetOrdinal("Region"))
            });
        }

        public async Task<OperationResult> ObtenerTodosAsync()
        {
            return await ExecuteReaderAsync("sp_ObtenerTodosClientes", cmd => { },
            reader => new ObtenerClienteDTO
            {
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Region = reader.GetString(reader.GetOrdinal("Region"))
            });
        }
    }
}
