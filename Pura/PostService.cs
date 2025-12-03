using Npgsql;

public class PostService
{
    private readonly NpgsqlDataSource _dataSource;

    public PostService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task CreatePostAsync(string title, string description, string price)
    {
        await using var cmd = _dataSource.CreateCommand(@"
        INSERT INTO posts (title, description, price, created_at) 
        VALUES (@title, @desc, @price, NOW()) 
        RETURNING id");

        cmd.Parameters.AddWithValue("title", title);
        cmd.Parameters.AddWithValue("desc", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("price", price);

        var newId = await cmd.ExecuteScalarAsync();   // ← this line proves it worked

        Console.WriteLine($"INSERT SUCCESSFUL → New post ID = {newId}");
    }
}