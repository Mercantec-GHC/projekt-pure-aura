// Services/CmdStorage.cs
using Npgsql;
using NpgsqlTypes;
using Microsoft.Extensions.Configuration;

namespace Pura.Services // Adjust namespace to match your project
{
    public class CmdStorage
    {
        private readonly string _connectionString;

        public CmdStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("NeonDb")
                ?? "Host=ep-dark-king-a92zz5wn-pooler.gwc.azure.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_N1Kf7gokGnEe; SSL Mode=VerifyFull; Channel Binding=Require;";
        }

        public async Task InsertProductAsync(string name, decimal price, string description)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = "INSERT INTO products (name, price, description) VALUES (@name, @price, @description)";
            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.Add("name", NpgsqlDbType.Text).Value = name ?? (object)DBNull.Value;
            cmd.Parameters.Add("price", NpgsqlDbType.Numeric).Value = price;
            cmd.Parameters.Add("description", NpgsqlDbType.Text).Value = description ?? (object)DBNull.Value;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}