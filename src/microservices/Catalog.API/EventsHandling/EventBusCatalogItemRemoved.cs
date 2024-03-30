using System;
using EventBus.Structures;

namespace Catalog.API.EventsHandling;

public class EventBusCatalogItemRemoved : EventBusPublisher
{
    public EventBusCatalogItemRemoved(string hostName, string userName, string password, int port)
        : base("Exchange.CatalogItemRemoved", hostName, userName, password, port)
    {
    }
}

