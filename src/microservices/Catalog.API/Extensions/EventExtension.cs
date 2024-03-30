using System;
using Catalog.API.EventsHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Extensions;

public static class EventExtension
{
	public static void AddEventExtension(this IServiceCollection services, IConfiguration configuration)
	{
        var _hostname = configuration["RabbitMQ:Hostname"]!;
        var _username = configuration["RabbitMQ:Username"]!;
        var _password = configuration["RabbitMQ:Password"]!;
        var _port = int.Parse(configuration["RabbitMQ:Port"]!);

        services.AddScoped(sp =>
            new EventBusCatalogItemCreated(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );

        services.AddScoped(sp =>
            new EventBusCatalogItemUpdated(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );

        services.AddScoped(sp =>
            new EventBusCatalogItemRemoved(
                hostName: _hostname,
                userName: _username,
                password: _password,
                port: _port)
            );
    }
}

