using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus;

public class EventBusConsumer : EventBusClient, IEventBusConsumer
{
    public string ConsumerTag { get; set; } = string.Empty;
    protected string _queueName = string.Empty;

    public EventBusConsumer(string hostName, string userName, string password, int port)
    {
        Initialize(hostName, userName, password, port);
    }

    public void Subscribe(string queueName, string exchange)
    {
        _exchangeName = exchange;

        AddQueue(exchange);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (a, b) =>
        {
            var text = Encoding.UTF8.GetString(b.Body.ToArray());
            //var messagebuffer = JsonSerializer.Serialize(objectToSend)); /
            Console.WriteLine($"Received '{text}' from '{b.ConsumerTag}'");
        };

        ConsumerTag = _channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer
            );
    }

    public void Unsubscribe()
    {
        if (ConsumerTag is null || _queueName is null || _exchangeName is null)
            throw new Exception("No queoe bounded yet");

        _channel.BasicCancel(ConsumerTag);

        _channel.QueueUnbind(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: string.Empty
            );

        _channel = null;
        _queueName = string.Empty;
        _exchangeName = string.Empty;
    }

    protected void AddQueue(string exchangeName)
    {
        if (_channel is null)
            throw new Exception("Client not initialized yer");

        _exchangeName = exchangeName;
        _queueName = _channel.QueueDeclare().QueueName;

        AddExchange(exchangeName);

        _channel.QueueBind(
            queue: _queueName,
            exchange: exchangeName,
            routingKey: string.Empty
            );
    }
}

