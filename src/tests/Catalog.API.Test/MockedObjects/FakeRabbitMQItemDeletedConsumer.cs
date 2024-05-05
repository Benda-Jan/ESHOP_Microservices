using Catalog.API.Write.EventsHandling;
using Catalog.Infrastructure;

namespace Catalog.API.Test.ComponentTests.MockedObjects;

public class FakeRabbitMQItemUpdatedConsumer : ItemUpdatedConsumer
{
    public FakeRabbitMQItemUpdatedConsumer(ICatalogRepository catalogRepository, FakeRabbitMQPublisherDeleted eventBusCatalogItemDeleted) 
        : base("", "", "", 0, catalogRepository, eventBusCatalogItemDeleted)
    {}

    public override void Subscribe()
    {
        Console.WriteLine($"Subscribed");
    }
}