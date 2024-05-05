using System;
using EventBus.Structures;

namespace Catalog.API.Write.EventsHandling;

public class EventBusCatalogItemDeleted : EventBusPublisher
{
    public EventBusCatalogItemDeleted(string hostName, string userName, string password, int port)
        : base("Exchange.CatalogItemDeleted", hostName, userName, password, port)
    {
    }
}

