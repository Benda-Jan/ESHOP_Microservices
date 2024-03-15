namespace EventBus.Exceptions;

public class ExchangeUndeclaredException : Exception
{
    public ExchangeUndeclaredException()
    {
    }

    public ExchangeUndeclaredException(string message)
        : base(message)
    {
    }

    public ExchangeUndeclaredException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

