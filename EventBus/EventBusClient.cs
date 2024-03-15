using System;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace EventBus;

public abstract class EventBusClient
{
    protected IModel? _channel;
    protected string _exchangeName = string.Empty;

    protected void Initialize(string hostName, string userName, string password, int port)
    {
        var factory = new ConnectionFactory { HostName = hostName, UserName = userName, Password = password, Port = port };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }

    protected void AddExchange(string exchangeName)
    {
        _channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Fanout
            );

        _exchangeName = exchangeName;
    }
}

