using Npgsql;
using Pura.Components;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // BEST PRACTICE: Get connection string from config (appsettings.json or user-secrets)
        var connectionString = builder.Configuration.GetConnectionString("NeonDb")
            ?? "Host=ep-dark-king-a92zz5wn.gwc.azure.neon.tech\r\nPort=5432\r\nDatabase=neondb\r\nUsername=neondb_owner\r\nPassword=...\r\nSslMode=Require;";

        // Create and register NpgsqlDataSource
        var dataSource = NpgsqlDataSource.Create(connectionString);
        builder.Services.AddSingleton(dataSource);  // This is correct

        // THIS IS THE LINE YOU WERE MISSING — ADD IT!
        builder.Services.AddScoped<PostService>();

        // Standard Blazor setup
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Optional: Test connection at startup (great for debugging)
        try
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT version()", conn);
            var version = await cmd.ExecuteScalarAsync();
            Console.WriteLine("PostgreSQL CONNECTED!");
            Console.WriteLine($"Version: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("DATABASE CONNECTION FAILED:");
            Console.WriteLine(ex.Message);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();

        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}