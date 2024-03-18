using Catalog.API.Extensions;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace Catalog.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddExtensions();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (app.Environment.IsEnvironment("Development") || app.Environment.IsEnvironment("Docker"))
        {
            app.ApplyMigrations();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CatalogContext>();

                var seeder = new CatalogContextSeeder(context);
                await seeder.SeedBrands();
                await seeder.SeedTypes();
            }

            app.UseSwagger(c => { c.RouteTemplate = "/swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));
        }

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.Run();
    }
}

