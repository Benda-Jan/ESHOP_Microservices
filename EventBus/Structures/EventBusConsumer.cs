using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using EventBus.Interfaces;
using EventBus.Exceptions;

namespace EventBus.Structures;

public class EventBusConsumer : EventBusClient, IEventBusConsumer
{
    public string ConsumerTag { get; set; } = string.Empty;
    protected string _queueName = string.Empty;

    public EventBusConsumer(string hostName, string userName, string password, int port)
    {
        Initialize(hostName, userName, password, port);
    }

    public virtual void Subscribe(string queueName, string exchange)
    {
        AddQueue(exchange);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (a, b) =>
        {
            var text = Encoding.UTF8.GetString(b.Body.ToArray());
            //var messagebuffer = JsonSerializer.Serialize(objectToSend)); /
            Console.WriteLine($"Received '{text}'");
        };

        ConsumerTag = _channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer
            );
    }

    public void Unsubscribe()
    {
        if (ConsumerTag is null || _queueName is null)
            throw new NotSubscribedException("Not subscribed to any queue yet");

        if (_exchangeName is null)
            throw new ExchangeUndeclaredException("No exchange specified");

        if (_channel is null)
            throw new ChannelNotCreatedException("No channel to cancel");

        _channel.QueueUnbind(
            queue: _queueName,
            exchange: _exchangeName,
            routingKey: string.Empty
            );

        _channel.BasicCancel(ConsumerTag);

        _channel = null;
        _queueName = string.Empty;
        _exchangeName = string.Empty;
        ConsumerTag = string.Empty;
    }

    protected void AddQueue(string exchangeName)
    {
        if (_channel is null)
            throw new ChannelNotCreatedException("Channel not created yet");

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

