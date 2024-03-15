using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus;

public abstract class EventBusPublisher : IEventBusPublisher, IDisposable
{
    private IModel? _channel;
    private readonly string _queueName;
    private readonly string _exchangeName;

    public EventBusPublisher(string queueName, string exchangeName)
    {
        _queueName = queueName;
        _exchangeName = exchangeName;
    }

    public virtual void Publish(object objectToSend)
    {
        Initialize();

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

    private void Initialize()
    {
        if (_channel is null)
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq-container", UserName = "user", Password = "mypass", Port = 5672};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false
                );

            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Headers
                );

            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: string.Empty
                );
        }
    }
   
}

