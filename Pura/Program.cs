using Npgsql;
using Pura.Components;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("NeonDb")
            ?? "Host=ep-dark-king-a92zz5wn.gwc.azure.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_N1Kf7gokGnEe;SslMode=Require";

        var dataSource = NpgsqlDataSource.Create(connectionString);
        builder.Services.AddSingleton(dataSource);

        builder.Services.AddScoped<NpgsqlConnection>(sp =>
            sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());

        builder.Services.AddScoped<PostService>();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

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
