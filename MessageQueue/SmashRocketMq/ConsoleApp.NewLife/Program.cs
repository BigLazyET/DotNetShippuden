using NewLife.Data;
using NewLife.RocketMQ;
using System.ComponentModel;

namespace ConsoleApp.NewLife
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var producer = new Producer
            {
                Topic = "foo",
                NameServerAddress = "localhost:9876",
                
            };
            producer.Start();

            Task.Run(async () =>
            {
                foreach (var item in Enumerable.Range(0, 4))
                {
                    await producer.PublishAsync($"send message {item}", "tag", "key");
                    await Task.Delay(3000);
                }
            });

            var consumer = new Consumer
            {
                Topic = "foo",
                Group = "fooGroup",
                NameServerAddress = "localhost:9876"
            };

            Task.Run(() =>
            {
                consumer.OnConsumeAsync = (queue, messages) =>
                {
                    Console.WriteLine($"receive messages: {queue.Topic}-{queue.QueueId}-{messages.Length}");
                    foreach (var message in messages)
                    {
                        Console.WriteLine($"receive message: {message.Tags}-{message.Flag}-{message.Keys}-{message.MsgId}-{message.BornTimestamp}-{message.BodyString}");
                    }

                    return Task.FromResult(true);
                };

                consumer.Start();
            });

            Console.ReadLine();
        }
    }
}