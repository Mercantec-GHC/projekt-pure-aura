// Services/CmdStorage.cs
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Pura.Services
{
    public partial class CmdStorage
    {
        private readonly string _connectionString;

        public CmdStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("NeonDb")
                ?? "Host=ep-dark-king-a92zz5wn-pooler.gwc.azure.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_N1Kf7gokGnEe; SSL Mode=VerifyFull; Channel Binding=Require;";
        }

        public async Task InsertJewelryAsync(string? name, decimal price, string? description)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = "INSERT INTO products (name, price, description) VALUES (@name, @price, @description)";
            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", (object?)name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@description", (object?)description ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        // let us make a method to get and display all products from the products table 

        public async Task<List<(int Id, string? Name, decimal Price, string? Description)>> GetAllProductsAsync()
        {
            var products = new List<(int Id, string? Name, decimal Price, string? Description)>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            const string sql = "SELECT id, name, price, description FROM products";
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32(0);
                var name = reader.IsDBNull(1) ? null : reader.GetString(1);
                var price = reader.GetDecimal(2);
                var description = reader.IsDBNull(3) ? null : reader.GetString(3);
                products.Add((id, name, price, description));
            }
            return products;
        }

        // convenience overload that accepts the shared model
        public async Task InsertJewelryAsync(JewelryModel jewelry)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                INSERT INTO products (name, price, description)
                VALUES (@name, @price, @description)";

            await using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@name", (object?)jewelry.Title ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@price", jewelry.Price);
            cmd.Parameters.AddWithValue("@description", (object?)jewelry.Description ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}