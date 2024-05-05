namespace EventBus.Interfaces;

public interface IEventBusConsumer
{
    void Subscribe();
    void Unsubscribe();
}

