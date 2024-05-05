using Cart.API.EventsHandling;

namespace Cart.API.Test.ComponentTests.MockedObjects;

public class FakeRabbitMQPublisherUpdated : EventBusCartItemUpdated
{
    
    public FakeRabbitMQPublisherUpdated() : base("", "", "", 0)
    {}
    
    public override void Publish(string message)
    {
        Console.WriteLine($"Message published: {message}");
    }

    public void Unsubscribe()
    {
        Console.WriteLine($"Unsubscribed");
    }
}