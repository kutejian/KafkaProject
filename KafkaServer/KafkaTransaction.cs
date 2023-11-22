using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaServer;

enum ProducerState
{
    MakingMessagesToAbort,
    MakingMessagesToCommit,
    InitState
}
public class KafkaTransaction1
{
    static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);
    public static void TransactionSend()
    {

        string brokerList = "134.175.91.184:9092,134.175.91.184:9093";
        // 不同的topic 的testtransactionalId  就不同
        string topicName = "test";
        // 写代码的时候，不一样的topic就写的不一样。。
        string transactionalId = "testtransactionalId234455555";
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            //幂等性
            EnableIdempotence = true,
            Acks = Acks.All,
            LingerMs = 10000,
            BatchNumMessages=1,
            MessageSendMaxRetries = 3,
            TransactionalId = transactionalId,
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();
        try
        {
            //初始化事务
            producer.InitTransactions(DefaultTimeout);
            var currentState = ProducerState.InitState;
            //开始事务
            producer.BeginTransaction();
            for (int i = 100; i < 110; i++)
            {
                var content = i.ToString();
                producer.Produce(
                   topicName, new Message<string, string> { Key = content, Value = content });
            }
            Console.WriteLine("输入");
            Console.ReadLine();
            //事务提交
            producer.CommitTransaction(DefaultTimeout);
        }
        catch (Exception ex)
        {
            //回滚 
            producer.AbortTransaction(DefaultTimeout);
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine($"done: {transactionalId}");
    }
}
