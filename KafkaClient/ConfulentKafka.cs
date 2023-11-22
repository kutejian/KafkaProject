using Confluent.Kafka;//生产环境上使用，但是我今天发现了一个它bug
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaClient;

class ConfulentKafka
{

    public static void Consumer(string brokerlist, List<string> topicname, string groupname)
    {

        var mode = "subscribe";
        var brokerList = brokerlist;
        var topics = topicname;
        Console.WriteLine($"Started consumer, Ctrl-C to stop consuming");
        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        switch (mode)
        {
            ///根据Kafka自己的记录偏移量来进行拉去消息
            case "subscribe":
                Run_Consume(brokerList, topics, groupname);
                break;
            ///生产环境慎用：Why？
            ///客户端通过自己保存的偏移量来进行拉取消息
            ///如果新增或者删除消费者，不会自动负载均衡
            ///如，有3个分区，搞了2个节点，发现不够用，又增加了一个
            case "manual":
                Run_ManualAssign(brokerList, topics, cts.Token);
                break;
            default:
                PrintUsage();
                break;
        }
    }
    /// <summary>
    ///  消费端拿到数据,告诉kafka数据我已经消费完了
    /// </summary>
    /// <param name="brokerList"></param>
    /// <param name="topics"></param>
    /// <param name="group"></param>
    public static void Run_Consume(string brokerList, List<string> topics, string group)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = brokerList,
            GroupId = group,

            EnableAutoCommit = false,
            //Latest = 0,表示消费者消费启动之后的数据。。。启动之前的数据消费不到
            //Earliest = 1,每次都从头开始消费没有消费过的数据
            //Error = 2
            AutoOffsetReset = AutoOffsetReset.Earliest,

            EnablePartitionEof = true,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range,
            FetchWaitMaxMs=1,   

            SessionTimeoutMs = 6000,
            MaxPollIntervalMs = 8000,
        };

        const int commitPeriod = 1;
        // 提交偏移量的时候,也可以批量去提交
        using (var consumer = new ConsumerBuilder<Ignore, string>(config)
            // 设置错误处理器，用于处理消费者发生错误时的情况
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            //分区撤销处理器 这个处理器用于在分区被撤销时执行一些逻辑。在这里，它简单地打印出分区的信息。
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                #region 指定分区消费
                // 之前可以自动均衡,现在不可以了 
                //List<TopicPartitionOffset> topics = new List<TopicPartitionOffset>();
                //// 我当前读取所有的分区里面的从10开始
                //foreach (var item in partitions)
                //{
                //	topics.Add(new TopicPartitionOffset(item.Topic, item.Partition, new Offset(10)));
                //}

                //return topics;
                #endregion
            })
            //SetPartitionsRevokedHandler
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                //新加入消费者的时候调用 
                Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
            }).Build()){
            //消费者订阅主题
            consumer.Subscribe(topics);
            try
            {
                // 死循环 拉取模式
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume();
                        if (consumeResult.IsPartitionEOF)
                        {
                            continue;
                        }
                        Console.WriteLine($": {consumeResult.TopicPartitionOffset}::{consumeResult.Message.Value}");
                        if (consumeResult.Offset % commitPeriod == 0)
                        {
                            try
                            {
                                // 程序员自己提交偏移量，数据自己已经处理完成了
                                //如果要解决数据不重复消费，把消息的主键写入redis
                                //Redis.call(Guid.....)	
                                Thread.Sleep(9000);
                                //consumer.Commit(consumeResult);
                                Console.WriteLine("提交");
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"Commit error: {e.Error.Reason}");
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                consumer.Close();
            }
        }
    }

    public static void Run_ManualAssign(string brokerList, List<string> topics, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = new Guid().ToString(),
            BootstrapServers = brokerList,
            EnableAutoCommit = true
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}")).SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                })
                .Build())
        {
            // 我指定读取那个一个,,哪一个偏移量
            consumer.Assign(topics.Select(topic => new TopicPartitionOffset(topic, 1, 6)).ToList());
            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: ${consumeResult.Message.Value}");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Closing consumer.");
                consumer.Close();
            }
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: .. <subscribe|manual> <broker,broker,..> <topic> [topic..]");
    }

}