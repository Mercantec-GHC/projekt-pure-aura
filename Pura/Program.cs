using Pura.Components;
using Npgsql;
using Microsoft.AspNetCore.Builder;

namespace Pura
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Define the connection string for the Neon Tech PostgreSQL database
            string connectionString =
                "Host=ep-dark-king-a92zz5wn-pooler.gwc.azure.neon.tech;" +
                "Database=neondb;" +
                "Username=neondb_owner;" +
                "Password=npg_N1Kf7gokGnEe;" +
                "Ssl Mode=Require;" +                 
                "Trust Server Certificate=true;";

            // Create and register the NpgsqlDataSource as a singleton
            var dataSource = NpgsqlDataSource.Create(connectionString);
            builder.Services.AddSingleton<NpgsqlDataSource>(dataSource);

            // TEST THE CONNECTION
            // Connection works so far, do not touch anything else here


            try // Try to open a connection and execute a simple query
            {
                await using var conn = await dataSource.OpenConnectionAsync();
                await using var cmd = new NpgsqlCommand("SELECT version()", conn);
                var version = await cmd.ExecuteScalarAsync();

                Console.WriteLine("PostgreSQL IS CONNECTED!");
                Console.WriteLine($"Version: {version}");
            }
            catch (Exception ex) // Catch any exceptions that occur during the connection attempt
            {
                Console.WriteLine("CONNECTION FAILED:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
            }


            // Register NpgsqlConnection to be created from the NpgsqlDataSource
            builder.Services.AddSingleton<NpgsqlConnection>(sp =>
                sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());

            // ... other service registrations (no changes needed from here)
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();
            
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

            // Important: Dispose the data source when the app shuts down
            // This ensures all connections are properly closed
            if (dataSource is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else if (dataSource is IDisposable disposable)
                disposable.Dispose();
        }
    }
}