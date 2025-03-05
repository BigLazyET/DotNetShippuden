// See https://aka.ms/new-console-template for more information
// 学习链接：https://www.cnblogs.com/whuanle/p/17837034.html
// 消费者，生产者，交换器，队列，路由键，绑定，Qos，拒绝接收
// 消息优先级，延迟队列，死信队列，DLX死信交换器，
// 消息持久化，消息确认机制，消息重试机制，队列TTL时间，消息TTL时间
// 事务机制，发送方确认机制

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmashRabbitMq;

// await MqBasicProducer();
// await MqBasicConsumer();
// await MqAckFasle();

await MqFooChainTestFanout();


Console.WriteLine("Ending...");
Console.ReadLine();

async Task MqBasicProducer()
{
    var factory = new ConnectionFactory { HostName = "localhost" };
    // 连接
    await using var connection = await factory.CreateConnectionAsync();
    // var connection = conTask.ConfigureAwait(false).GetAwaiter().GetResult();
    // 通道
    await using var channel = await connection.CreateChannelAsync();
    // 声明队列
    // queue：队列的名称。
    // durable：设置【队列】是否持久化。持久化的队列会存盘，在服务器重启的时候可以保证不丢失相关信息。（这里与发送消息的时候消息的持久化properties.Persistent = true区分开）
    // 如果队列本身不持久化（durable: false），即使消息是持久化的（Persistent = true），在服务器重启时，消息也会丢失，因为队列不存在了
    // exclusive 设置是否排他。如果一个队列被声明为排他队列，该队列仅对首次声明它的连接可见，并在连接断开时自动删除。
    // 该配置是基于 IConnection 的，同一个 IConnection 创建的不同通道 (IModel) ，也会遵守此规则。
    // autoDelete：设置是否自动删除。自动删除的前提是至少有一个消费者连接到这个队列，之后所有与这个队列连接的消费者都断开时，才会自动删除。
    // argurnents: 设置队列的其他一些参数，如队列的消息过期时间等。
    await channel.QueueDeclareAsync(queue: "fooq", durable: false, exclusive: false, autoDelete: false,
        arguments: null);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "fooq", mandatory: false,
        body: Encoding.UTF8.GetBytes("hello, foo"));

    var array = Enumerable.Range(0, 10).ToArray();
    var memory = new ReadOnlyMemory<int>(array);

    foreach (var i in array)
    {
        await channel.BasicPublishAsync(
            exchange: string.Empty, // 默认交换器
            routingKey: "FooQueue", // 路由键
            mandatory: false,
            body: Encoding.UTF8.GetBytes($"测试{i}")
        );
    }
}

async Task MqBasicConsumer()
{
    var factory = new ConnectionFactory { HostName = "localhost" };
    // 连接
    var connection = await factory.CreateConnectionAsync();
    // 通道
    var channel = await connection.CreateChannelAsync();
    var consumer = new AsyncEventingBasicConsumer(channel);
    var cid = 0;
    consumer.ReceivedAsync += ConsumerOnReceivedAsync;

    await channel.BasicConsumeAsync(queue: "FooQueue", autoAck: false, consumer: consumer);
    return;


    async Task ConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var message = Encoding.UTF8.GetString(@event.Body.Span);
        Console.WriteLine($"message receive: {message}");
        cid++;
        if (cid % 2 == 0)
            await channel.BasicAckAsync(@event.DeliveryTag, multiple: false);
    }
}

async Task MqAckFasle()
{
    var connectionFactory = new ConnectionFactory { HostName = "localhost" };
    var connection = await connectionFactory.CreateConnectionAsync();
    var channel = await connection.CreateChannelAsync();

    foreach (var item in Enumerable.Range(0, 10))
    {
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "FooQueue", mandatory: false,
            body: Encoding.UTF8.GetBytes($"测试AckFasle{item}"));
    }

    var cid = 0;
    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += ConsumerOnReceivedAsync;
    await channel.BasicConsumeAsync(queue: "FooQueue", autoAck: false, consumer: consumer);
    return;

    async Task ConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var message = Encoding.UTF8.GetString(@event.Body.Span);
        Console.WriteLine($"MqAckFalse message {message}");
        cid++;

        // deliveryTag: ea.DeliveryTag：这是消息的唯一标识符，用于确认具体哪条消息已被处理。ea.DeliveryTag 通常是 BasicDeliverEventArgs 对象中的一个属性。
        // multiple: false：这个参数表示是否批量确认。如果设为 false，则只确认指定的单条消息。如果设为 true，则会确认所有小于或等于 deliveryTag 的消息。
        if (cid % 2 == 0)
            await channel.BasicAckAsync(deliveryTag: @event.DeliveryTag, multiple: false);

        // if (cid == 30)
        //     await channel.DisposeAsync();
    }
}

async Task MqFooChainTestFanout()
{
    var mqExchange = new MqFooChain();
    await mqExchange.InitAsync();
    mqExchange.WithNodeDeclare(config =>
    {
        config.AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "fooEx"))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "barEx"))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "bazEx"))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Queue, "fooq"))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Queue, "bazq"));
    }).WithNodeBind(new List<Node>
    {
        new() { BindFrom = "fooEx", BindTo = "fooq", Type = NodeBindType.Queue },
        new() { BindFrom = "fooEx", BindTo = "bazq", Type = NodeBindType.Queue },
        new() { BindFrom = "barEx", BindTo = "bazEx", Type = NodeBindType.Exchange },
        new() { BindFrom = "bazEx", BindTo = "bazq", Type = NodeBindType.Queue },
    });

    await mqExchange.BuildAsync();

    await mqExchange.TestPublishAsync("fooEx", string.Empty, 10);
    await mqExchange.TestPublishAsync("barEx", string.Empty, 100);
}

async Task MqFooChainTestTopic()
{
    var mqExchange = new MqFooChain();
    await mqExchange.InitAsync();
    mqExchange.WithNodeDeclare(config =>
    {
        config.AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "b1", ExchangeType.Topic))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "b2", ExchangeType.Topic))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "b3", ExchangeType.Topic))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Queue, "q1"))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Queue, "q2"));
    }).WithNodeBind(new List<Node>
    {
        new() { BindFrom = "b1", BindTo = "q1", Type = NodeBindType.Queue, RoutingKey = "fruit.*" },
        new() { BindFrom = "b2", BindTo = "q2", Type = NodeBindType.Queue, RoutingKey = "veg.#" },
        new() { BindFrom = "b2", BindTo = "b3", Type = NodeBindType.Exchange, RoutingKey = "fruit.apple.#" },
        new() { BindFrom = "b3", BindTo = "q1", Type = NodeBindType.Queue, RoutingKey = "fruit.apple.red.*.*" },
    });

    await mqExchange.BuildAsync();

    await mqExchange.TestPublishAsync("b1", count: 2, attach: true, routingKey: "fruit.");
    await mqExchange.TestPublishAsync("b1", count: 2, attach: true, routingKey: "fruit.banan.");
    await mqExchange.TestPublishAsync("b1", count: 2, attach: true, routingKey: "meat.");
    await mqExchange.TestPublishAsync("b2", count: 3, attach: true, routingKey: "veg.");
    await mqExchange.TestPublishAsync("b2", count: 3, attach: true, routingKey: "veg.tomato.");
    await mqExchange.TestPublishAsync("b2", count: 5, attach: true, routingKey: "fruit.apple");
    await mqExchange.TestPublishAsync("b2", count: 5, attach: true, routingKey: "fruit.apple.");
    await mqExchange.TestPublishAsync("b2", count: 5, attach: true, routingKey: "fruit.apple.red.");
    await mqExchange.TestPublishAsync("b2", count: 5, attach: true, routingKey: "fruit.apple.yellow.");
    await mqExchange.TestPublishAsync("b2", count: 5, attach: true, routingKey: "fruit.apple.red.yellow.");
}

async Task MqDlxTest()
{
    var mqExchange = new MqFooChain();
    await mqExchange.InitAsync();
    mqExchange.WithNodeDeclare(config =>
    {
        config.AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "c1_bak", ExchangeType.Direct))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Exchange, "c1", ExchangeType.Direct,
                Arguments: new Dictionary<string, string> { { "x-dead-letter-exchange", "c1_bak" } }))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Queue, "c1q"))
            .AddOption(new NodeDeclareOption(NodeDeclareType.Queue, "c1_bakq"));
    }).WithNodeBind(new List<Node>
    {
        new() { BindFrom = "c1", BindTo = "c1q", Type = NodeBindType.Queue, RoutingKey = "car.order" },
        new() { BindFrom = "c1_bak", BindTo = "c1_bakq", Type = NodeBindType.Queue, RoutingKey = "car.order.delay" }
    });

    await mqExchange.BuildAsync();

    await mqExchange.TestPublishAsync("c1", "car.order", 20);
    await mqExchange.TestPublishAsync("c1", "car.order", 20);
    
    var cid = 0;
    mqExchange.consumer.ReceivedAsync += ConsumerOnReceivedAsync;
    await mqExchange.consumerChannel.BasicConsumeAsync("c1q", false, mqExchange.consumer);
    return;
    
    async Task ConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        if (cid % 2 == 0)
            await mqExchange.consumerChannel.BasicRejectAsync(@event.DeliveryTag, false);
        cid++;
    }
}