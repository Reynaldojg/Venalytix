using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Venalytix.Persistence.Context
{
    public class VenalytixDbContext : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;

        public VenalytixDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");
        }

        /// Devuelve una conexión abierta a la base de datos. Si no existe, la crea.
        public IDbConnection GetConnection()
        {
            if (_connection == null)
                _connection = new SqlConnection(_connectionString);

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }
}
