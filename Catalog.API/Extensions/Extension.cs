using System;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Npgsql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Catalog.API.EventsHandling;
using EventBus;

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

        //var myKey = builder.Configuration["MyKey"];
        //builder.Services.AddSingleton<IService3>(sp => new Service3(myKey));

        builder.Services.AddScoped(sp =>
            new EventBusCatalogItemCreated(
                queueName: "Queue.CatalogItemCreated",
                exchangeName: "Exchange.CatalogItemCreated",
                hostName: builder.Configuration["RabbitMq:Hostname"] ?? "localhost",
                userName: "user",
                password: "mypass",
                port:5672)
            );
    }
}

