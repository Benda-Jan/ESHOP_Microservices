using System;
using Identity.Entities;
using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Identity.API.Extensions;

public static class ContextExtension
{
	public static void AddContextExtension(this IServiceCollection services, IConfiguration configuration)
	{
        var connectionString = configuration["PostgreSQL:ConnectionString"];
        var dbPassword = configuration["PostgreSQL:DbPassword"];

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString) { Password = dbPassword };

        services.AddDbContext<UserContext>(options =>
            options.UseNpgsql(connectionStringBuilder.ConnectionString)
            );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddTransient<IEncryptor, Encryptor>();
    }
}

