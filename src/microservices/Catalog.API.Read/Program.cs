using Catalog.API.Read.Extensions;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Catalog.Infrastructure.Services;
using Extensions;

namespace Catalog.API.Read;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddContextExtension<CatalogContext>(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisCache:ConnectionString"];
        });

        builder.Services.AddTransient<ICacheService, RedisCacheService>();
        builder.Services.AddTransient<ICatalogRepository, CatalogRepository>();

        builder.Services.AddHealthCheckExtensions(builder.Configuration);

        builder.Services.AddCors(options =>
            options.AddPolicy("newPolicy", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (app.Environment.IsEnvironment("Development"))
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
        }

        app.UseSwagger(c => { c.RouteTemplate = "/swagger/{documentName}/swagger.json"; });
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));

        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseCors("newPolicy");

        app.Run();
    }
}

