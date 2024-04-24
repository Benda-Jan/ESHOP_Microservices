using Cart.Infrastructure;

namespace Cart.API.EventsHandling;

public static class EventExtension
{
    public static void AddEventExtension(this IServiceCollection services, IConfiguration configuration)
    {
        var _hostname = configuration["RabbitMQ:Hostname"]!;
        var _username = configuration["RabbitMQ:Username"]!;
        var _password = configuration["RabbitMQ:Password"]!;
        var _port = int.Parse(configuration["RabbitMQ:Port"]!);

        services.AddTransient(sp =>
            new EventBusCartItemUpdated(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );

        services.AddSingleton<IHostedService>(sp => 
        {
            var scope = sp.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
            return new ItemUpdatedConsumer(_hostname, _username, _password, _port, repository);
        });

        services.AddSingleton<IHostedService>(sp => 
        {
            var scope = sp.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
            return new ItemDeletedConsumer(_hostname, _username, _password, _port, repository);
        });
    }
}