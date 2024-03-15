using System;

namespace EventBus;

public interface IEventBusConsumer
{
    void Subscribe(string queueName, string exchange);
    void Unsubscribe();
}

