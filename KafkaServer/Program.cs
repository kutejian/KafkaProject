using KafkaServer;

while (1 == 1)
{
    Console.WriteLine("请输入发送的内容");
    var message = Console.ReadLine();
    //代码要连接kafka
    string brokerList = "134.175.91.184:9092,134.175.91.184:9093";
    ConfulentKafka.Produce(brokerList, "test", message);
}