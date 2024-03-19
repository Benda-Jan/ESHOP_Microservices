using System;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Npgsql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Catalog.API.EventsHandling;
using Catalog.API.Services;


namespace Catalog.API.Extensions;

public static class Extensions
{
	public static void AddExtensions(this IHostApplicationBuilder builder)
	{
        var connectionString = builder.Configuration["PostgreSql:ConnectionString"];
        var dbPassword = builder.Configuration["PostgreSql:DbPassword"];
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString) { Password = dbPassword };

        builder.Services.AddControllers();

        builder.Services.AddDbContext<CatalogContext>(options =>
            options.UseNpgsql(connectionStringBuilder.ConnectionString)
        );

        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIs", Version = "v1" });
        });

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

        builder.Services.AddHealthChecks()
            .AddNpgSql(
            connectionString: connectionString,
            healthQuery: "SELECT 1",
            name: "NpgSqlHealtCheck",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] {"sql", "npgsql", "healthchecks"});

        builder.Services.AddScoped<EventBusCatalogItemCreated>(sp =>
            new EventBusCatalogItemCreated(
                hostName: builder.Configuration["RabbitMQ:Hostname"] ?? "localhost",
                userName: builder.Configuration["RabbitMQ:Username"] ?? "user",
                password: builder.Configuration["RabbitMQ:Password"] ?? "",
                port:5672)
            );

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisCache:ConnectionString"];
        });

        builder.Services.AddTransient<ICacheService, RedisCacheService>();
    }
}

