namespace EventBus.Interfaces;

public interface IEventBusPublisher
{
    void Publish(string message);
}

