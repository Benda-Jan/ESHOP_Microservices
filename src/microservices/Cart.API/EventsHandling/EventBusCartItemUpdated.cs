using System;
using EventBus.Structures;

namespace Cart.API.EventsHandling;

public class EventBusCartItemUpdated : EventBusPublisher
{
    public EventBusCartItemUpdated(string hostName, string userName, string password, int port)
        : base("Exchange.CartItemUpdated", hostName, userName, password, port)
    {
    }
}

