using System;
using EventBus;

namespace Catalog.API.EventsHandling;

public class EventBusCatalogItemCreated : EventBusPublisher
{
    public EventBusCatalogItemCreated(string queueName, string exchangeName, string hostName, string userName, string password, int port)
        : base(hostName, userName, password, port)
    {
        AddExchange(exchangeName);
    }
}

