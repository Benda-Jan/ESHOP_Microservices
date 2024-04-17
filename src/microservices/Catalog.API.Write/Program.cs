using Catalog.API.Write.Extensions;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.OpenApi.Models;
using JwtLibrary;
using System.Text;
using Catalog.Infrastructure.Services;
using Extensions;

namespace Catalog.API.Write;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddContextExtension<CatalogContext>(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddSwaggerServiceExtension();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisCache:ConnectionString"];
        });

        builder.Services.AddTransient<ICacheService, RedisCacheService>();
        builder.Services.AddTransient<ICatalogRepository, CatalogRepository>();

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddEventExtension(builder.Configuration);

        var rabbitConnection = new StringBuilder();
        rabbitConnection
            .Append("amqp://")
            .Append(builder.Configuration["RabbitMQ:Username"] ?? "user")
            .Append(":")
            .Append(builder.Configuration["RabbitMQ:Password"] ?? "")
            .Append("@")
            .Append(builder.Configuration["RabbitMQ:Hostname"] ?? "localhost")
            .Append("/");

        builder.Services.AddHealthChecks()
            .AddNpgSql(
                connectionString: builder.Configuration["PostgreSQL:ConnectionString"] ?? "",
                tags: new string[] { "CatalogDatabase", "PostgreSQL" }
            ).AddRedis(
                builder.Configuration["RedisCache:ConnectionString"] ?? "",
                tags: new string[] { "RedisCacheCatalogAPI", "Redis" }
            ).AddRabbitMQ(
                rabbitConnectionString: rabbitConnection.ToString(),
                tags: new string[] { "EventBus", "RabbitMQ" }
            );

        builder.Services.AddCors(options =>
            options.AddPolicy("newPolicy", policy => policy/*.WithOrigins("localhost")*/.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

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

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseCors("newPolicy");

        app.Run();
    }
}

