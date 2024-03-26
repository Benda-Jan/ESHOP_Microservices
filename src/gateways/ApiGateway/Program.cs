
using JwtLibrary;
using AuthorizationMiddleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Configuration.AddJsonFile("Ocelot.Development.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile("Ocelot.Docker.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot(builder.Configuration);

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddTransient<JwtMiddleware>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<JwtMiddleware>();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseOcelot();

        app.MapControllers();

        app.Run();
    }
}

