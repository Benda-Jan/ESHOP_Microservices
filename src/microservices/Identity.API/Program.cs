using HealthChecks.UI.Client;
using Extensions;
using Identity.Entities;
using Identity.Infrastructure;
using Identity.Infrastructure.Data;
using JwtLibrary;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Identity.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);      
        
        // Add services to the container.
        builder.Services.AddContextExtension<UserContext>(builder.Configuration);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IEncryptor, Encryptor>();

        builder.Services.AddHealthChecks()
            .AddNpgSql(
                connectionString: builder.Configuration["PostgreSQL:ConnectionString"] ?? "",
                tags: new string[] { "IdentityDatabase", "PostgreSQL" }
            );

        builder.Services.AddCors(options =>
            options.AddPolicy("newPolicy", policy => policy/*.WithOrigins("localhost")*/.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsEnvironment("Development") || app.Environment.IsEnvironment("Docker"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

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

