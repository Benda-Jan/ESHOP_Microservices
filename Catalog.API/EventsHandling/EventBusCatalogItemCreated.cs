using System;
using EventBus;

namespace Catalog.API.EventsHandling;

public class EventBusCatalogItemCreated : EventBusPublisher
{
    public EventBusCatalogItemCreated(string queueName, string exchangeName)
        : base(queueName, exchangeName)
    {

    }
}

