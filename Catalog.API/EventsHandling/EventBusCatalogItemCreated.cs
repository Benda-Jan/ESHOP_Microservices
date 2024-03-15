using System;
using EventBus.Structures;

namespace Catalog.API.EventsHandling;

public class EventBusCatalogItemCreated : EventBusPublisher
{
    public EventBusCatalogItemCreated(string exchangeName, string hostName, string userName, string password, int port)
        : base(hostName, userName, password, port)
    {
        AddExchange(exchangeName);
    }
}

