using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KafkaServer;

class ConfulentKafka
{
    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="event"></param>
    public static async Task Produce(string brokerlist, string topicname, string content)
    {
        string brokerList = brokerlist;
        string topicName = topicname;
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            Acks = Acks.All,
            EnableIdempotence = true,

            MessageSendMaxRetries = 3,
            Partitioner = Partitioner.Random,
            EnableDeliveryReports = true,

            LingerMs = 3000, 
            BatchNumMessages = 2,
        };

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {

            Console.WriteLine("\n-----------------------------------------------------------------------");
            Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
            Console.WriteLine("-----------------------------------------------------------------------");

          var deliveryReport = await producer.ProduceAsync(
              topicName, new Message<string, string>
              { Key = new Random().Next(1, 10).ToString(), Value = content });
          Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
        }
    }
}