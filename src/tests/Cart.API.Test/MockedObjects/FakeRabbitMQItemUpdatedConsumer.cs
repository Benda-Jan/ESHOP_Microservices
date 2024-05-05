using Cart.API.EventsHandling;
using Cart.Infrastructure;

namespace Cart.API.Test.ComponentTests.MockedObjects;

public class FakeRabbitMQItemUpdatedConsumer : ItemUpdatedConsumer
{
    public FakeRabbitMQItemUpdatedConsumer(ICartRepository cartRepository) 
        : base("", "", "", 0, cartRepository)
    {}

    public override void Subscribe()
    {
        Console.WriteLine($"Subscribed");
    }
}