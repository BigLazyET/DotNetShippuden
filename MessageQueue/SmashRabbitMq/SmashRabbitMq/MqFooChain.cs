using System.Text;
using RabbitMQ.Client;

namespace SmashRabbitMq;

public sealed class MqFooChain : IDisposable, IAsyncDisposable
{
    private IConnection _connection;
    private IChannel _producer;
    private IChannel _consumer;

    private SinglyLinkedList<IList<Node>> _nodeLinkedList;
    private NodeDeclareConfig _nodeDeclareConfig; // 都可以通过依赖注入，但是为了简单直接新建实例

    public async Task InitAsync()
    {
        var connectionFactory = new ConnectionFactory { HostName = "localhost" };
        _connection = await connectionFactory.CreateConnectionAsync();
        _producer = await _connection.CreateChannelAsync();
        _consumer = await _connection.CreateChannelAsync();

        _nodeLinkedList = new SinglyLinkedList<IList<Node>>();
        _nodeDeclareConfig = new NodeDeclareConfig();
    }

    public async Task TestPublishAsync(string exchangeName, string routingKey, int count)
    {
        foreach (var item in Enumerable.Range(0, count))
        {
            await _producer.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, mandatory: false,
                body: Encoding.UTF8.GetBytes($"{exchangeName}_{item}"));
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
                await _producer.ExchangeDeclareAsync(exchange: option.NodeName, type: ExchangeType.Fanout,
                    durable: option.Durable, autoDelete: option.AutoDelete);
            else
                await _producer.QueueDeclareAsync(queue: option.NodeName, exclusive: option.Exclusive,
                    durable: option.Durable, autoDelete: option.AutoDelete);
        }

        foreach (var nodes in _nodeLinkedList)
        {
            foreach (var node in nodes)
            {
                if (node.Type == NodeBindType.Exchange)
                    await _producer.ExchangeBindAsync(source: node.BindFrom, destination: node.BindTo,
                        routingKey: node.RoutingKey);
                else
                    await _producer.QueueBindAsync(exchange: node.BindFrom, queue: node.BindTo,
                        routingKey: node.RoutingKey);
            }
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
        _producer.Dispose();
        _consumer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _producer.DisposeAsync();
        await _consumer.DisposeAsync();
    }
}

public record struct NodeDeclareOption(
    NodeDeclareType DeclareType,
    string NodeName,
    bool Durable = false,
    bool Exclusive = false,
    bool AutoDelete = false);

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