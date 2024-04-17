namespace EventBus.Exceptions;

public class ChannelNotCreatedException : Exception
{
    public ChannelNotCreatedException()
    {
    }

    public ChannelNotCreatedException(string message)
        : base(message)
    {
    }

    public ChannelNotCreatedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

