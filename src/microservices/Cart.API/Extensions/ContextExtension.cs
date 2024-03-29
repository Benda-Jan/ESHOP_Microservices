using System;
using Cart.Infrastructure;
using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Cart.API.Extensions;

public static class ContextExtension
{
	public static void AddContextExtension(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration["PostgreSQL:ConnectionString"];
		var password = configuration["PostgreSQL:DbPassword"];

		NpgsqlConnectionStringBuilder builder = new(connectionString) { Password = password};

		services.AddDbContext<CartContext>(options =>
			options.UseNpgsql(builder.ConnectionString));

        //services.AddHealthChecks()
        //    .AddNpgSql(
        //    connectionString: connectionString ?? "",
        //    healthQuery: "SELECT 1",
        //    name: "CartContextHelathCheck",
        //    failureStatus: HealthStatus.Unhealthy,
        //    tags: new[] { "sql", "npgsql", "healthchecks" });

        services.AddScoped<ICartRepository, CartRepository>();
    }
}

