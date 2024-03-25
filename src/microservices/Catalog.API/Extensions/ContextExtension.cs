
using System;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Catalog.API.Extensions;

public static class ContextExtension
{
	public static void AddContextExtension(this IServiceCollection services, IConfiguration configuration)
	{
        var connectionString = configuration["PostgreSql:ConnectionString"];
        var dbPassword = configuration["PostgreSql:DbPassword"];
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString) { Password = dbPassword };

        services.AddDbContext<CatalogContext>(options =>
            options.UseNpgsql(connectionStringBuilder.ConnectionString)
        );

        services.AddHealthChecks()
            .AddNpgSql(
            connectionString: connectionString,
            healthQuery: "SELECT 1",
            name: "NpgSqlHealtCheck",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "sql", "npgsql", "healthchecks" });
    }
}

