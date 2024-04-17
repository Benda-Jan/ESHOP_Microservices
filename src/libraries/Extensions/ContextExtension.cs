using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Extensions;

public static class ContextExtension
{
    public static void AddContextExtension<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : DbContext
	{
        var connectionString = configuration["PostgreSql:ConnectionString"];
        var dbPassword = configuration["PostgreSql:DbPassword"];
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString) { Password = dbPassword };

        services.AddDbContext<TDbContext>(options =>
            options.UseNpgsql(connectionStringBuilder.ConnectionString)
        );
    }
}
