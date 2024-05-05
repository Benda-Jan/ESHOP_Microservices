using RabbitMQ.Client;

namespace EventBus.Structures;

public abstract class EventBusClient : IDisposable
{
    protected IModel? _channel;
    protected string _exchangeName;

    protected EventBusClient(string exchangeName)
    {
       _exchangeName = exchangeName;
    }

    protected void Initialize(string hostName, string userName, string password, int port)
    {
        var factory = new ConnectionFactory { HostName = hostName, UserName = userName, Password = password, Port = port };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        AddExchange(_exchangeName);
    }

    private void AddExchange(string exchangeName)
    {
        _channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Fanout
            );
    }

    public void Dispose()
    {
        if (_channel != null)
            _channel.Abort();
    }
}

