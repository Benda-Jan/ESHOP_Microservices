using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus;

public class EventBusConsumer : IEventBusConsumer
{
    private IModel? _channel;
    public string QueueName { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string ConsumerTag { get; set; } = string.Empty;

    public void Publish(string queueName, string exchangeName, object objectToSend)
    {
        if (_channel is null)
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "user", Password = "mypass" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false
                );

            _channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: string.Empty
                );

            _channel.ExchangeDeclare(
                exchange: exchangeName,
                type: ExchangeType.Headers
                );
        }

        byte[] messagebuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(objectToSend));
        _channel.BasicPublish(
            exchange: exchangeName,
            routingKey: string.Empty,
            body: messagebuffer
            );
    }

    public void Subscribe(string queueName, string exchange)
    {
        QueueName = queueName;
        Exchange = exchange;

        var factory = new ConnectionFactory { HostName = "rabbitmq-container", UserName = "user", Password = "mypass", Port = 5672 };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.BasicQos(0, 1, false);

        _channel.ExchangeDeclare(
            exchange: Exchange,
            type: ExchangeType.Headers
            );

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false
            );

        _channel.QueueBind(
            queue: QueueName,
            exchange: Exchange,
            routingKey: string.Empty
            );

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (a, b) =>
        {
            var text = Encoding.UTF8.GetString(b.Body.ToArray());
            //var messagebuffer = JsonSerializer.Serialize(objectToSend)); /
            Console.WriteLine($"Received '{text}' from '{b.ConsumerTag}'");
        };

        ConsumerTag = _channel.BasicConsume(
            queue: QueueName,
            autoAck: true,
            consumer: consumer
            );
    }

    public void Unsubscribe()
    {
        if (_channel is null)
            return;

        _channel.BasicCancel(ConsumerTag);

        _channel.QueueUnbind(
            queue: QueueName,
            exchange: Exchange,
            routingKey: string.Empty
            );

        _channel = null;
        QueueName = string.Empty;
        Exchange = string.Empty;
    }
}

