namespace EventBus.Exceptions;

public class NotSubscribedException : Exception
{
    public NotSubscribedException()
    {
    }

    public NotSubscribedException(string message)
        : base(message)
    {
    }

    public NotSubscribedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

