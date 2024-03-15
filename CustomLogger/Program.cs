using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using EventBus;

namespace CustomLogger;

class Program
{
    static void Main(string[] args)
    {
        var client = new EventBusConsumer()
        {
            QueueName = "Queue.Test",
            Exchange = "Exchange.Test"
        };

        client.Subscribe("Queue.Test", "Exchange.Test");


        Console.WriteLine($"Press button to unsibscribe - {client.ConsumerTag}");

        Console.ReadKey();
        Console.WriteLine("Unsibscribed");

        client.Unsubscribe();

        Console.ReadKey();
    }
}

