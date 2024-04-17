using System;
using EventBus.Structures;

namespace Catalog.API.Write.EventsHandling;

public class EventBusCatalogItemCreated : EventBusPublisher
{
    public EventBusCatalogItemCreated(string hostName, string userName, string password, int port)
        : base("Exchange.CatalogItemCreated", hostName, userName, password, port)
    {
    }
}

