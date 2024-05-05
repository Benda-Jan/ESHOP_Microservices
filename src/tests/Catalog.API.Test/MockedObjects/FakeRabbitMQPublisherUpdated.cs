using Catalog.API.Write.EventsHandling;

namespace Catalog.API.Test.ComponentTests.MockedObjects;

public class FakeRabbitMQPublisherUpdated : EventBusCatalogItemUpdated
{
    
    public FakeRabbitMQPublisherUpdated() : base("", "", "", 0)
    {}
    
    public override void Publish(string message)
    {
        Console.WriteLine($"Subscribed to: {message}");
    }

    public void Unsubscribe()
    {
        Console.WriteLine($"Unsubscribed");
    }
}