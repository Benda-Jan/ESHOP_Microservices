

using System.Text;
using System.Text.Json;
using Catalog.Entities.DbSet;
using Catalog.Infrastructure;
using EventBus.Structures;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Catalog.API.Write.EventsHandling;

public class ItemUpdatedConsumer : EventBusConsumer, IHostedService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly EventBusCatalogItemRemoved _eventBusCatalogItemRemoved;
    public ItemUpdatedConsumer(string hostname, string username, string password, int port, ICatalogRepository catalogRepository, EventBusCatalogItemRemoved eventBusCatalogItemRemoved) 
        : base(hostname, username, password, port)
    {
        _catalogRepository = catalogRepository;
        _eventBusCatalogItemRemoved = eventBusCatalogItemRemoved;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Subscribe("Exchange.CartItemUpdated");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override void Subscribe(string exchange)
    {
        AddQueue(exchange);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (a, b) =>
        {
            var jsonObject = Encoding.UTF8.GetString(b.Body.ToArray());
            var cartItem = JsonSerializer.Deserialize<CatalogItemDeserializer>(jsonObject);
            try{
                if (cartItem != null)
                {
                    var catalogItem = await _catalogRepository.GetItemById(cartItem.CatalogItemId);
                    if (catalogItem != null)
                    {
                        catalogItem.AvailableStock -= cartItem.Quantity;
                        await _catalogRepository.UpdateItem(catalogItem);
                    } 
                    else
                    {
                        var serializeCartItem = new CatalogItemSerializer
                        {
                            Id = cartItem.CatalogItemId
                        };
                        _eventBusCatalogItemRemoved.Publish(JsonSerializer.Serialize(serializeCartItem));
                    } 
                }
                    
            } catch(Exception ex)
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