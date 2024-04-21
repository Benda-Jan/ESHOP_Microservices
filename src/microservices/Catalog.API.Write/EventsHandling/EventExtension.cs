using Catalog.API.Write.EventsHandling;

namespace Catalog.API.Write.EventsHandling;

public static class EventExtension
{
	public static void AddEventExtension(this IServiceCollection services, IConfiguration configuration)
	{
        var _hostname = configuration["RabbitMQ:Hostname"]!;
        var _username = configuration["RabbitMQ:Username"]!;
        var _password = configuration["RabbitMQ:Password"]!;
        var _port = int.Parse(configuration["RabbitMQ:Port"]!);

        services.AddTransient(sp =>
            new EventBusCatalogItemCreated(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );

        services.AddTransient(sp =>
            new EventBusCatalogItemUpdated(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );

        services.AddTransient(sp =>
            new EventBusCatalogItemRemoved(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );
    }
}

