using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using EventBus.Interfaces;
using EventBus.Exceptions;

namespace EventBus.Structures;

public abstract class EventBusPublisher : EventBusClient, IEventBusPublisher
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly int _port;
    protected EventBusPublisher(string exchangeName, string hostName, string userName, string password, int port) : base(exchangeName)
    {
        _hostName = hostName;
        _userName = userName;
        _password = password;
        _port = port;
    }

    public virtual void Publish(string message)
    {
        Initialize(_hostName, _userName, _password, _port);

        if (_exchangeName is null)
            throw new ExchangeUndeclaredException("No exchange specified");

        byte[] messagebuffer = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: string.Empty,
            body: messagebuffer
            );
    }
}

