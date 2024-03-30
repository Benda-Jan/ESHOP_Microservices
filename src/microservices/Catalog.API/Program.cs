using Catalog.API.Extensions;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Catalog.API.EventsHandling;
using Catalog.API.Services;
using JwtLibrary;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIs", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT token with the prefix Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
        });

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        builder.Services.AddScoped<EventBusCatalogItemCreated>(sp =>
            new EventBusCatalogItemCreated(
                hostName: builder.Configuration["RabbitMQ:Hostname"] ?? "localhost",
                userName: builder.Configuration["RabbitMQ:Username"] ?? "user",
                password: builder.Configuration["RabbitMQ:Password"] ?? "",
                port: 5672)
            );

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisCache:ConnectionString"];
        });

        builder.Services.AddTransient<ICacheService, RedisCacheService>();

        builder.Services.AddContextExtension(builder.Configuration);

        builder.Services.AddJwtAuthentication(builder.Configuration);

        var rabbitConnection = new StringBuilder();
        rabbitConnection
            .Append("amqp://")
            .Append(builder.Configuration["RabbitMQ:Username"] ?? "user")
            .Append(":")
            .Append(builder.Configuration["RabbitMQ:Password"] ?? "")
            .Append("@")
            .Append(builder.Configuration["RabbitMQ:Hostname"] ?? "localhost")
            .Append("/");
        // "amqp://user:mypass@localhost/"

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

        if (app.Environment.IsEnvironment("Development") || app.Environment.IsEnvironment("Docker"))
        {
           // app.ApplyMigrations();
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

