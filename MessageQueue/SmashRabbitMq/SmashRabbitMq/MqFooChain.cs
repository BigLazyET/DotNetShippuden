using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SmashRabbitMq;

public sealed class MqFooChain : IDisposable, IAsyncDisposable
{
    private IConnection _producerConnection;
    private IConnection _consumerConnection;
    private IChannel _producerChannel;
    public IChannel consumerChannel;
    public AsyncEventingBasicConsumer consumer;

    private SinglyLinkedList<IList<Node>> _nodeLinkedList;
    private NodeDeclareConfig _nodeDeclareConfig; // 都可以通过依赖注入，但是为了简单直接新建实例

    public async Task InitAsync()
    {
        var connectionFactory = new ConnectionFactory { HostName = "localhost" };
        _producerConnection = await connectionFactory.CreateConnectionAsync();
        _consumerConnection = await connectionFactory.CreateConnectionAsync();
        _producerChannel = await _producerConnection.CreateChannelAsync();
        consumerChannel = await _consumerConnection.CreateChannelAsync();
        consumer = new AsyncEventingBasicConsumer(consumerChannel);

        _nodeLinkedList = new SinglyLinkedList<IList<Node>>();
        _nodeDeclareConfig = new NodeDeclareConfig();
    }

    public async Task TestPublishAsync(string exchangeName, string routingKey, int count, bool attach = false)
    {
        foreach (var item in Enumerable.Range(0, count))
        {
            if (!string.IsNullOrWhiteSpace(routingKey) && attach)
                routingKey = $"{routingKey}.{item}";
            var body = string.IsNullOrWhiteSpace(routingKey) ?  Encoding.UTF8.GetBytes($"{exchangeName}-{item}") : Encoding.UTF8.GetBytes($"{exchangeName}-{routingKey}");
            await _producerChannel.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, mandatory: false,
                body: body);
        }
    }

    public MqFooChain WithNodeBind(IList<Node> nodes)
    {
        _nodeLinkedList.AddLast(nodes);
        return this;
    }

    public MqFooChain WithNodeDeclare(Action<NodeDeclareConfig> configure)
    {
        configure.Invoke(_nodeDeclareConfig);
        return this;
    }

    public async Task BuildAsync()
    {
        foreach (var option in _nodeDeclareConfig.Options)
        {
            if (option.DeclareType == NodeDeclareType.Exchange)
                await _producerChannel.ExchangeDeclareAsync(exchange: option.NodeName, type: option.ExchangeType,
                    durable: option.Durable, autoDelete: option.AutoDelete);
            else
                await _producerChannel.QueueDeclareAsync(queue: option.NodeName, exclusive: option.Exclusive,
                    durable: option.Durable, autoDelete: option.AutoDelete);
        }

        foreach (var nodes in _nodeLinkedList)
        {
            foreach (var node in nodes)
            {
                if (node.Type == NodeBindType.Exchange)
                    await _producerChannel.ExchangeBindAsync(source: node.BindFrom, destination: node.BindTo,
                        routingKey: node.RoutingKey);
                else
                    await _producerChannel.QueueBindAsync(exchange: node.BindFrom, queue: node.BindTo,
                        routingKey: node.RoutingKey);
            }
        }
    }

    public void Dispose()
    {
        _producerConnection.Dispose();
        _consumerConnection.Dispose();
        _producerChannel.Dispose();
        consumerChannel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _producerConnection.DisposeAsync();
        await _consumerConnection.DisposeAsync();
        await _producerChannel.DisposeAsync();
        await consumerChannel.DisposeAsync();
    }
}

public record struct NodeDeclareOption(
    NodeDeclareType DeclareType,
    string NodeName,
    string ExchangeType = ExchangeType.Fanout,
    bool Durable = false,
    bool Exclusive = false,
    bool AutoDelete = false,
    IDictionary<string,string>? Arguments = null);

public class NodeDeclareConfig
{
    public List<NodeDeclareOption> Options = new();
}

public static class ExchangeBindConfigExtensions
{
    public static NodeDeclareConfig AddOption(this NodeDeclareConfig config, NodeDeclareOption option)
    {
        config.Options.Add(option);
        return config;
    }
}