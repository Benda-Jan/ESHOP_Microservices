using System.Text;
using System.Text.Json;
using Cart.Entities.DbSet;
using Cart.Infrastructure;
using EventBus.Structures;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cart.API.EventsHandling;

public class ItemDeletedConsumer : EventBusConsumer, IHostedService
{
    private readonly ICartRepository _cartRepository;
    public ItemDeletedConsumer(string hostname, string username, string password, int port, ICartRepository cartRepository) 
        : base("Exchange.CatalogItemDeleted", hostname, username, password, port)
    {
        _cartRepository = cartRepository;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Subscribe();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override void Subscribe()
    {
        AddQueue();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (a, b) =>
        {
            var jsonObject = Encoding.UTF8.GetString(b.Body.ToArray());
            var cartItem = JsonSerializer.Deserialize<CartItemDeserializer>(jsonObject);
            try
            {
                if (cartItem != null)
                    await _cartRepository.DeleteCatalogItem(cartItem.Id);
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        };

        ConsumerTag = _channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer
            );
    }
}