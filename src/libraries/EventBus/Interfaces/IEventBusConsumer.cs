namespace EventBus.Interfaces;

public interface IEventBusConsumer
{
    void Subscribe(string queueName, string exchange);
    void Unsubscribe();
}

