using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using EventBus.Interfaces;
using EventBus.Exceptions;

namespace EventBus.Structures;

public abstract class EventBusPublisher : EventBusClient, IEventBusPublisher, IDisposable
{
    public EventBusPublisher(string hostName, string userName, string password, int port)
    {
        Initialize(hostName, userName, password, port);
    }

    public virtual void Publish(string message)
    {
        if (_exchangeName is null)
            throw new ExchangeUndeclaredException("No exchange specified");

        byte[] messagebuffer = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: string.Empty,
            body: messagebuffer
            );
    }

    public void Dispose()
    {
        // ??
    }
}

