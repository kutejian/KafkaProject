using KafkaClient;


string brokerList = "134.175.91.184:9092,134.175.91.184:9093";

//原理+实战
var topics = new List<string> { "test" };
Console.WriteLine("请输入组名称");
string groupname = Console.ReadLine();
ConfulentKafka.Consumer(brokerList, topics, groupname);