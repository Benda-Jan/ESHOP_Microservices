using Extensions;
using Cart.Infrastructure;
using HealthChecks.UI.Client;
using JwtLibrary;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Cart.Infrastructure.Data;
using Cart.API.EventsHandling;
using Cart.API.Payment;

namespace Cart.API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddContextExtension<CartContext>(builder.Configuration);  

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerServiceExtension();

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddScoped<ICartRepository, CartRepository>();

        builder.Services.AddTransient<IPaymentClient>(sp => 
        {
            var scope = sp.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
            return new PaymentClient(builder.Configuration["PaymentCient:ConnectionString"] ?? "", repository);
        });

        builder.Services.AddHealthChecks()
            .AddNpgSql(
                connectionString: builder.Configuration["PostgreSQL:ConnectionString"] ?? "",
                tags: new string[] { "CartDatabase", "PostgreSQL" }
            );

        builder.Services.AddCors(options =>
            options.AddPolicy("newPolicy", policy => policy/*.WithOrigins("localhost")*/.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        
        builder.Services.AddEventExtension(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
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
