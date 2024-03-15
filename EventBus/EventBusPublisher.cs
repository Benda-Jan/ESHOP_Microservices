using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus;

public abstract class EventBusPublisher : EventBusClient, IEventBusPublisher, IDisposable
{
    public EventBusPublisher(string hostName, string userName, string password, int port)
    {
        Initialize(hostName, userName, password, port);
    }

    public virtual void Publish(object objectToSend)
    {
        if (_exchangeName is null)
            throw new Exception("No exchange specified");

        byte[] messagebuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(objectToSend));
        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: string.Empty,
            body: messagebuffer
            );
    }

    public void Dispose()
    {
        Console.WriteLine("Disposed");
    }
}

