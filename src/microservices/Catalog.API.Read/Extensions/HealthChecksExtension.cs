
using System.Text;

namespace Catalog.API.Read.Extensions;

public static class HealthChecksExtension
{
    public static void AddHealthCheckExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConnection = new StringBuilder();
        rabbitConnection
            .Append("amqp://")
            .Append(configuration["RabbitMQ:Username"] ?? "user")
            .Append(":")
            .Append(configuration["RabbitMQ:Password"] ?? "")
            .Append("@")
            .Append(configuration["RabbitMQ:Hostname"] ?? "localhost")
            .Append("/");

        services.AddHealthChecks()
            .AddNpgSql(
                connectionString: configuration["PostgreSQL:ConnectionString"] ?? "",
                tags: new string[] { "CatalogDatabase", "PostgreSQL" }
            ).AddRedis(
                configuration["RedisCache:ConnectionString"] ?? "",
                tags: new string[] { "RedisCacheCatalogAPI", "Redis" }
            ).AddRabbitMQ(
                rabbitConnectionString: rabbitConnection.ToString(),
                tags: new string[] { "EventBus", "RabbitMQ" }
            );
    }
}