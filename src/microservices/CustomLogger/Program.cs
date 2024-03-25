using EventBus.Structures;

namespace CustomLogger;

class Program
{
    static void Main(string[] args)
    {
        var client = new EventBusConsumer("localhost", "user", "mypass", 5672);

        client.Subscribe("Queue.CatalogItemCreated", "Exchange.CatalogItemCreated");

        Console.WriteLine($"Press button to unsibscribe - {client.ConsumerTag}");

        Console.ReadKey();
        Console.WriteLine("Unsubscribed");

        client.Unsubscribe();

        Console.ReadKey();
    }
}

