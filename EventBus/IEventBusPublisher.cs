using System;

namespace EventBus;

public interface IEventBusPublisher
{
    void Publish(object objectToSend);
}

