namespace EventBus.Interfaces;

public interface IEventBusConsumer
{
    void Subscribe(string exchange);
    void Unsubscribe();
}

