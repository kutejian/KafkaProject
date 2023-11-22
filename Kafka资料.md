## [Kafka](https://kafka.apache.org/10/documentation.html#gettingStarted)

### 消息中间件对比：

既然讲过 RabbitMQ，Why Kafka？

```C#
// Q:RabbitMQ的镜像集群解决了什 么问题？
// A:高可用，但如何实现负载均衡？但无法解决负载均衡问题
```

![image-20230625110551445](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625110551445.png)

RabbitMQ——HA Proxy——Vip IP——这样我就可以对RabbitMQ做负载均衡了

Real？Why？

镜像集群也好，普通集群也罢，镜像集群就是实现数据冗余保证高可用的，普通集群更别说了。只能是多节点支持，底层还是自己去互相请求获取数据。

在RabbitMQ底层里面有一个Leader/主节点，我们在上层可以做负载均衡，这步是没问题的，但是如果这里面左边是主，那么最后请求还是访问给到左边的节点里面去。

![image-20230625110841441](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625110841441.png)

所以，一些大厂就是这么玩儿的，既然一套搞不了，那我搞个两三套，中间搞好了通讯。

但是这样明眼人一看就知道不太好，而且也不符合我们大数据的“精神”（要求），而且单体的架构也确实满足不了需求，此外，如果没有厉害的技术人员，这套架构也搞不起来的。

---

于是乎，Kafka来了，Kafka是Java开发的。

![](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625112501389.png)

官方的压测图（吞吐量）：1S能处理多少任务/请求，很早的图了，这个当时给的是36M，Kafka是600多M



![image-20230625112651960](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625112651960.png)

官方的压测图（延迟性）：RabbitMQ的延迟很低，延迟越低代表数据实时性越高。

RabbitMQ是最佳的，Kafka次之，所以，要看需求和选型。而且要看数据量，毕竟Kafka的吞吐量是600M+

如果你只是小项目，但对于数据的丢失性容忍度较低，实时性要求较高，选择RabbitMQ

如果你是有着海量数据并发，要求快速消费，那无脑入Kafka，这已经是工业的首选。

----

Q：那，Kafka没有缺点了吗？

A：太重了，环境搭建ZK，到Kafka，再到资源使用，要比RabbitMQ重的多，但好像也有轻量级版本了，目前没有研究

---

### Kafka介绍：

Kafka是由[Apache软件基金会](https://baike.baidu.com/item/Apache软件基金会/2912769?fromModule=lemma_inlink)开发的一个开源流处理平台，由[Scala](https://baike.baidu.com/item/Scala/2462287?fromModule=lemma_inlink)和[Java](https://baike.baidu.com/item/Java/85979?fromModule=lemma_inlink)编写。Kafka是一种高[吞吐量](https://baike.baidu.com/item/吞吐量/157092?fromModule=lemma_inlink)的分布式[发布订阅](https://baike.baidu.com/item/发布订阅/22695073?fromModule=lemma_inlink)消息系统，它可以处理消费者在网站中的所有[动作流](https://baike.baidu.com/item/动作流/15738974?fromModule=lemma_inlink)数据。 这种动作（[网页浏览](https://baike.baidu.com/item/网页浏览/12731498?fromModule=lemma_inlink)，搜索和其他用户的行动）是在现代网络上的许多[社会功能](https://baike.baidu.com/item/社会功能/8238266?fromModule=lemma_inlink)的一个关键因素。 这些数据通常是由于吞吐量的要求而通过处理日志和日志聚合来解决。 对于像[Hadoop](https://baike.baidu.com/item/Hadoop?fromModule=lemma_inlink)一样的日志数据和[离线分析](https://baike.baidu.com/item/离线分析/12730463?fromModule=lemma_inlink)系统，但又要求[实时处理](https://baike.baidu.com/item/实时处理/9897789?fromModule=lemma_inlink)的限制，这是一个可行的解决方案。

**Kafka的目的是通过[Hadoop](https://baike.baidu.com/item/Hadoop?fromModule=lemma_inlink)的并行加载机制来统一线上和离线的消息处理，也是为了通过集群来提供实时的消息**。

目前Net中玩儿这个还不太多，但已经是应用于大数据，IOT物联网中很广泛了

---

### 消息队列：

![image-20230625135840657](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625135840657.png)

```sh
## 没有队列时候，注册信息后需要发送短信，如瞬时量较大，可能会把发短信平台搞崩
## 有消息队列后，异步解耦，流量削峰
```

---

### 装配环境：

- Net6
- VS2022
- Docker

---

### Kafka知识：

<u>**Kafka的代码很简单，但有大量的原理知识，这个比代码要关键的多，需要更多理解，面试也都是这些东西。**</u>

#### Zookeeper

![image-20230625140357091](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625140357091.png)

Kafka重就重在这里，ZK（ZooKeeper）大数据的很多组件都在使用它

Zookeeper是什么呢，它相当于是一个大脑中枢，比如：数据一致性，Leader，同步发送等，都是Zookeeper来提供

也可以说是一个数据库，但它里面有很多功能是方便我们去使用的，也可以用作分布式锁，但建议还是跑去Redis那边用吧，哈哈哈

- Broker：Kafka节点
- Producers：生产者
- Consumers：消费者

![image-20230625142838836](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625142838836.png)

- 正常选举：往ZK写一条信息，比如有3个节点，宕机1个，还有2个，那么这两个同时往ZK写一条Node信息，ZK中只能保持一条，这样就是谁写的最快，谁是老大。
- 剩下一个：没得选了，就你了
- 全都挂了：等....

所以，ZK在Kafka中的最大作用就是它可以帮助我们管理Kafka来进行Leader选举，监控集群状态。

---

#### Kafka 术语的概念：

```sh
## Producer:
    生产者：生产数据，就是我们客户端代码，写数据的
## Consumer：
    消费者:负责处理kafka服务里面消息，写代码
## Consumer Group/Consumers：
 	轮询：消费者每一个处理一条，轮排
 	广播：一条信息，多个消费者同时处理，比如，日志，写了文本日志，还写数据库日志
 	
 	消费者组：就是kafka独特处理轮询还是广播。
## Broker：
	kafka服务，一个Broker可以创建多个topic
## Topic：
	我们代码中写入broker主题，一个kafka集群里面可以有多个Topic,为了区分业务和模块使用
## Partition（负载均衡）：
	就是把一个topic的信息分成几个区，利用多个节点，把多个分区，放在不同节点上面，实现负载均衡，kafka内部实现的。。 
## Replica（高可用）：
	副本，就是防止主节点宕机，然后数据丢失了，保证高可用。。
## Offset：
	偏移量，就是消息的主键，生产者负责写数据的时候，写进去然后偏移量，消费者消费数据，知道数据消费到什么地方，不要重复消费
```

---

#### Kafka副本：

![image-20230625144258182](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230625144258182.png)

Broker1和Borker2，是Kafka里面的2个节点

有一个TopicA的主题，TopicA上有2个分区，分别是分区1和分区0

如：分区1的Leader和Follower都在一个节点，100个请求分流给我们的Broker1和2，意外发生时当某个Broker其宕机就全部完蛋，所以分2个节点做备份，也就是主从的意思。保证高可用。然后尽快做故障恢复，恢复我们的集群

---

### Kafka集群搭建：

#### 单节点集群架构演示：

```sh
version: '2'

services:
  zoo1:
    image: wurstmeister/zookeeper
    restart: unless-stopped
    hostname: zoo1
    ports:
      - "6181:2181"
    container_name: zookeeper_kafka

  # kafka version: 1.1.0
  # scala version: 2.12
  kafka1:
    image: wurstmeister/kafka
    ports:
      - "9092:9092"
    environment:
      # 这里要写自己的ip
      KAFKA_ADVERTISED_HOST_NAME: 192.168.10.10
      # 本地可以写zoo1：2181，本地内网络，对应的zoo1，也可以写成192.168.10.60：6181
      KAFKA_ZOOKEEPER_CONNECT: "zoo1:2181"
      KAFKA_BROKER_ID: 1
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
      KAFKA_CREATE_TOPICS: "stream-in:1:1,stream-out:1:1"
    depends_on:
      - zoo1
    container_name: kafka
```

```shell
# 启动
docker-compose up -d
```

---

#### Kafka集群操作：

ZK的集群安装：

```sh
version: '3.4'

services:
  zoo1:
    image: zookeeper:3.4
    restart: always
    hostname: zoo1
    container_name: zoo1
    ports:
    - 2184:2181
    volumes:
    - "/mydata/kafka/clusterkafka/zoo1/data:/data"
    - "/mydata/kafka/clusterkafka/zoo1/datalog:/datalog"
    environment:
      ZOO_MY_ID: 1
      ZOO_SERVERS: server.1=zoo1:2888:3888 server.2=zoo2:2888:3888 server.3=zoo3:2888:3888
    networks:
      kafka:
        ipv4_address: 172.19.0.11

  zoo2:
    image: zookeeper:3.4
    restart: always
    hostname: zoo2
    container_name: zoo2
    ports:
    - 2185:2181
    volumes:
    - "/mydata/kafka/clusterkafka/zoo2/data:/data"
    - "/mydata/kafka/clusterkafka/zoo2/datalog:/datalog"
    environment:
      ZOO_MY_ID: 2
      ZOO_SERVERS: server.1=zoo1:2888:3888 server.2=0.0.0.0:2888:3888 server.3=zoo3:2888:3888
    networks:
      kafka:
        ipv4_address: 172.19.0.12

  zoo3:
    image: zookeeper:3.4
    restart: always
    hostname: zoo3
    container_name: zoo3
    ports:
    - 2186:2181
    volumes:
    - "/mydata/kafka/clusterkafka/zoo3/data:/data"
    - "/mydata/kafka/clusterkafka/zoo3/datalog:/datalog"
    environment:
      ZOO_MY_ID: 3
      ZOO_SERVERS: server.1=zoo1:2888:3888 server.2=zoo2:2888:3888 server.3=0.0.0.0:2888:3888
    networks:
      kafka:
        ipv4_address: 172.19.0.13

networks:
  kafka:
    external:
      name: kafka
```

Kafka集群安装：

```sh
version: '3.4'

services:
  kafka1:
    image: wurstmeister/kafka
    restart: always
    hostname: kafka1
    container_name: kafka1
    privileged: true
    ports:
    - 9092:9092
    environment:
      KAFKA_ADVERTISED_HOST_NAME: kafka1
      KAFKA_LISTENERS: PLAINTEXT://kafka1:9092
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://192.168.10.10:9092
      KAFKA_ADVERTISED_PORT: 9092
      KAFKA_ZOOKEEPER_CONNECT: zoo1:2181,zoo2:2181,zoo3:2181
    volumes:
    - /data/volume/kfkluster/kafka1/logs:/kafka
    external_links:
    - zoo1
    - zoo2
    - zoo3
    networks:
      kafka:
        ipv4_address: 172.19.0.14

  kafka2:
    image: wurstmeister/kafka
    restart: always
    hostname: kafka2
    container_name: kafka2
    privileged: true
    ports:
    - 9093:9093
    environment:
      KAFKA_ADVERTISED_HOST_NAME: kafka2
      KAFKA_LISTENERS: PLAINTEXT://kafka2:9093
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://192.168.10.10:9093
      KAFKA_ADVERTISED_PORT: 9093
      KAFKA_ZOOKEEPER_CONNECT: zoo1:2181,zoo2:2181,zoo3:2181
    volumes:
    - /data/volume/kfkluster/kafka2/logs:/kafka
    external_links:
    - zoo1
    - zoo2
    - zoo3
    networks:
      kafka:
        ipv4_address: 172.19.0.15

  kafka3:
    image: wurstmeister/kafka
    restart: always
    hostname: kafka3
    container_name: kafka3
    privileged: true
    ports:
    - 9094:9094
    environment:
      KAFKA_ADVERTISED_HOST_NAME: kafka3
      KAFKA_LISTENERS: PLAINTEXT://kafka3:9094
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://192.168.10.10:9094
      KAFKA_ADVERTISED_PORT: 9094
      KAFKA_ZOOKEEPER_CONNECT: zoo1:2181,zoo2:2181,zoo3:2181
    volumes:
    - /data/volume/kfkluster/kafka3/logs:/kafka
    external_links:
    - zoo1
    - zoo2
    - zoo3
    networks:
      kafka:
        ipv4_address: 172.19.0.16

networks:
  kafka:
    external:
      name: kafka
```

Kafka的界面管理：

```sh
version: "3.4"

services:
  kafka-manager:
    image: sheepkiller/kafka-manager:latest
    restart: always
    container_name: kafka-manager
    hostname: kafka-manager
    ports:
     - 9000:9000
    environment:
     ZK_HOSTS: zoo1:2181,zoo2:2181,zoo3:2181
     KAFKA_BROKERS: kafka1:9092,kafka2:9092,kafka3:9092
     APPLICATION_SECRET: letmein
     KM_ARGS: -Djava.net.preferIPv4Stack=true
    networks:
     kafka:
      ipv4_address: 172.19.0.17
networks:
  kafka:
    external:
      name: kafka
```

```sh
# 启动
docker-compose -f zk.yml -f kafka.yml -f kafkamanage.yml up -d
```

```sh
#docker-compose 默认会创建网络，不需要手动执行
#如果执行错误,则需要删除其他的network
#docker network ls
#docker network name/id
docker network create --driver bridge --subnet 172.19.0.0/16 --gateway 172.19.0.1 kafka
```

```sh
# 查看kafka版本
find / -name \*kafka_\* | head -1 | grep -o '\kafka[^\n]*' 
```

```sh
# 访问管理页面
192.168.10.10:9000
# 连接ZK
192.168.10.10:2184,192.168.10.10:2185,192.168.10.10:2186
# 其他提示填写2个
```

---

### Net连接Kafka：

#### 老规矩，第一步，导包：

```C#
  <ItemGroup>
    <PackageReference Include="Confluent.Kafka" Version="2.1.1" />
  </ItemGroup>
```

```C#
 public static async Task Produce(string brokerlist, string topicname, string content)
    {
        string brokerList = brokerlist;
        string topicName = topicname;
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            // ack ---注意注意注意。。KafKa的ACK是生产者的，Rabbitmq是消费者的
            // 代码封装的原因，消费端也可以写，实际没有用到===
            // 数据写入保存机制--- 保证数据不丢失，而且会影响到我们性能，
            // 越高级别数据不丢失，则写入的性能越差
            Acks = Acks.All,
            // 幂等性，保证不会丢数据。。 
            EnableIdempotence = true,



            // 只要上面的两个要求符合一个，则后台线程立刻马上把数据推送给broker
            // 失败条件：可以看到发送的偏移量，如果没有偏移量，则就是没有写成功
            MessageSendMaxRetries = 3,//补偿重试，发送失败了则重试 
                                      //  Partitioner = Partitioner.Random


            LingerMs = 10000, //信息发送完，多久吧数据发送到broker里面去
            //BatchNumMessages = 2,//控制条数，当内存的数据条数达到了，立刻马上发送过去
        };

        using (var producer = new ProducerBuilder<string, string>(config)
            //.SetValueSerializer(new CustomStringSerializer<string>())
            //	.SetStatisticsHandler((o, json) =>
            //{
            //	Console.WriteLine("json");
            //	Console.WriteLine(json);
            //})
            .Build())
        {

            Console.WriteLine("\n-----------------------------------------------------------------------");
            Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
            Console.WriteLine("-----------------------------------------------------------------------");
            try
            {
                // 建议使用异步，传说性能比较好
                // Key 主要是做负载均衡，注意： 比如，有三个节点，一个topic，创建了三个分区。。一个节点一个分区， 
                // 但是，如果你在写入的数据的时候，没有写key,这样会导致，所有的数据存放到一个分区上面。。。
                //ps：如果用了分区，打死也要写key .根据自己的业务，可以提前配置好，
                // key的随机数，可以根据业务，搞一个权重，如果节点的资源不一样，合理利用资源，可以去写一个
                var deliveryReport = await producer.ProduceAsync(
                    topicName,
                    new Message<string, string> { Key = new Random().Next(1, 10).ToString(), Value = content }
                    );
                Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");

            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
            }
        }
```

- 分区数量最好和节点数量一致，但副本数量根据自己情况来，不一定每个节点都要有一个，压力山大。

---

#### 面试题1：

```sh
# kafka的数据写入是有顺序的吗？
如果是1个分区，则是有顺序的
如果是多个分区，则只能保证一个分区里面是有顺序的
```

---

#### ACK和数据可靠性：

![image-20230627200527991](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627200527991.png)

```C#
// ack ---注意注意注意。。KafKa的ACK是生产者的，Rabbitmq是消费者的
// 代码封装的原因，消费端也可以写，实际没有用到===
// 数据写入保存机制--- 保证数据不丢失，而且会影响到我们性能，
// 越高级别数据不丢失，则写入的性能越差
```

解决方案：

|            方案             |                      优点                      |                      缺点                       |
| :-------------------------: | :--------------------------------------------: | :---------------------------------------------: |
| 半数以上完成同步，就发送ACK |                     延迟低                     | 选举Leader时，容忍N台节点的故障，需要2N+1个副本 |
|   全部完成同步，才发送ACK   | 选举Leader时，容忍N台节点的故障，需要N+1个副本 |                     延迟高                      |

---

#### Ack的确认流程和实现过程：

![image-20230627202137811](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627202137811.png)

引发思考，如果我们有一个节点一直没有确认ACK，那是否整个集群会受牵连？

![image-20230627202945646](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627202945646.png)

---

#### 面试题2：

```sh
# leader收到数据，所有 follower都开始同步数据，但有一个 follower，因为某种故障，迟迟不能与 leader进行同步，那 leader就要一直等下去，直到它完成同步，才能发送 ack。这个问题怎么解决呢
ISR：同步的副本集合，维护当前还在存活的办事副本，解决防止当我们一个副本出现问题的时候，不能正常返回ACK。

## 继续：
# 那通过什么来维护ISR，是根据网络？还是根据心跳？还是根据备份数据量。。条数？时间？
1.根据条数
根据备份节点存放的数据条数和主节点数据条数做一个比较，设置一个差值，超过这个配置差值，就认为副本节点不行了，然后从ISR中移除，当数据备份上来了后，再重新回来，纳入我们ISR集合

2.根据时间
根据时间来判断，多长时间备份节点和主节点没有联系，超过配置时间，则代表备份节点挂掉了，可以从ISR排除出去，当心跳跟得上了，回来吧~

# 随机应变：
# Kafka是个高吞吐量的消息队列，有批量发送数据的功能，每次发送可以发送大量消息出去，可配置。所以如果根据条数，需要把设置值设置合理，不能同步数据小于一次发送的数据量之差。如果小于，则副本节点会经常移除，因为基于这种考虑，所以Kafka的开发者选择使用第二种方式处理
# 但是，选择心跳还有一个问题，那就是数据不一致，消费端看到的数据和生产端不一致。后面慢慢说
```

![image-20230627204726697](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627204726697.png)

```sh
ACK=0  # 性能最高，但是当写入Leader后返回ACK，如没有落盘，宕机，则数据丢失。 日志系统，IOT设备状态都选择它
```

![image-20230627204758401](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627204758401.png)

```sh
ACK=1  # 性能中等 Leader落盘后，返回ACK。如这时候 Leader宕机，新的Leader备份同步，则数据丢失，只能后期人工参与方式恢复。比较鸡肋
```

![image-20230627205304533](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627205304533.png)

```sh
ACK=1  # 性能最低 当Leader和所有的副本全部落盘后才返回ACK，肯定不会数据丢失，但有一个问题！
```

![image-20230627205513381](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230627205513381.png)

```sh
ACK=-1  # 性能最低 当数据都备份，此刻Leader宕机，没有返回ACK的情况下，新的副本产生Leader，生产者进行了补偿重试，数据重复。所以，幂等性很关键。结合幂等性，发送数据 服务端验证唯一性即可。如果之前发送过，就不在接收，不会报错，也只会存在一个。

# 性能差，差在哪里？
1.数据备份慢，所有副本备份完成后才会发送ACK
2.如果开启幂等性，则服务端接收数据的时候，要验证数据的唯一性。验证，就浪费时间。

## 实际使用Kafka，用第一个的最多。如果你要用 ACK= -1，那建议RabbitMQ。

设备/日志，都会发送大量的消息过来，使用Kafka，0性能最高。    下 => 上
平台下发指令，RabbitMQ    							   上 => 下
```

---

#### 幂等性：

```C#
 var config = new ProducerConfig
        {
            // ack ---注意注意注意。。KafKa的ACK是生产者的，Rabbitmq是消费者的
            // 代码封装的原因，消费端也可以写，实际没有用到===
            // 数据写入保存机制--- 保证数据不丢失，而且会影响到我们性能，
            // 越高级别数据不丢失，则写入的性能越差
            Acks = Acks.All,
     
            // 幂等性，保证不会丢数据。。 
            EnableIdempotence = true,
        };

```

---

#### 事务：

- 幂等性，开启服务端验证，

- 单个分区：消息的MsgId,客户端Id。
- 多个分区：消息的MsgId,客户端Id，+分区Id（Key）
- 有一个发生改变，即会重复，但很少见

如何解决？事务

```C#
// 多个分区，保证幂等性，开始事务，
// 原因就是因为一个topic如果有三个分区，则这三个分区，总共有三个leader..
// 比如，两个leader是正常，然后一个leader出现问题，它的备份节点替换它。。
// 相当于sql 保证写三个表==实现acid  

// 发送数据给leader(正常--- )
// 发送过去了，leader宕机--没有备份节点。。。
// 告诉之前两个leader把数据回滚。。。（通过事务发送的时候，服务端在保存数据的，多了一个标识===（0））
// 三个都正常，然后提交--（改标识，， 2个ok,其中一个不ok ）
string transactionalId = "testtransactionalId234455555";
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            //幂等性
            EnableIdempotence = true,
            Acks = Acks.All,
            //LingerMs = 10000,
            //BatchNumMessages=1,
            //MessageSendMaxRetries = 3,
            TransactionalId = transactionalId,
            // 客户端的唯一的标识,, 分区id , 消息的唯一标识
            //transactionalId
            //Partitioner = Partitioner.Murmur2Random  //你的把数据写完 

        };

using var producer = new ProducerBuilder<string, string>(config).Build();
        try
        {
            producer.InitTransactions(DefaultTimeout);
            var currentState = ProducerState.InitState;
            producer.BeginTransaction();
            for (int i = 100; i < 110; i++)
            {
                var content = i.ToString();
                producer.Produce(
                   topicName, new Message<string, string> { Key = content, Value = content });
            }
            Console.WriteLine("输入");
            Console.ReadLine();
            //  这些数据都是可以使用的
            //var content1 = "107"; 
            //producer.Produce(
            //	   topicName, new Message<string, string> { Key = content1, Value = content1 });
            producer.CommitTransaction(DefaultTimeout);
        }
        catch (Exception ex)
        {
            //回滚 
            producer.AbortTransaction(DefaultTimeout);
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine($"done: {transactionalId}");
```

---

### Kafka高效读写：

Kafka的吞吐量是RabbitMQ的十几倍，why？

- 批量发送
- 顺序读写
- 零拷贝

---

#### 批量发送：

![image-20230628093531293](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230628093531293.png)

```sh
# 1.时间：时间到了，批量发送。
# 2.条数：条数够了，批量发送。
以上2个条件，满足其1，发送给Kafka
```

---

#### 顺序读写：

```sh
# Mongodb——日志刷盘，顺序读写
# Kafka——数据落盘，顺序读写——只有新增——7天自动删除
# ES——数据日志追加，顺序读写
# Redis——AOF数据落盘日志追加，顺序读写
# Mysql——主从复制，顺序读写

顺序读写 业务场景：
关系型数据库，不适合。数据有增删改，不能浪费空间。有空位置就可以存，读不能顺序读，有条件的。
Why Redis或者其他Nosql也可以做增删改？日志追加。

不同场景，使用不同的思维和技术
```

![image-20230628104206703](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230628104206703.png)

- 追加新增
- 时间戳标识
- 日志文件合并

---

#### 零拷贝（卷）：

Java卷来卷去，问的深入。为了应对面试，来说这个事情。

![image-20230628104815246](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230628104815246.png)

```sh
用户空间：
# 微信——录屏，都有自己的进程，目的就是为了保护我们计算机，保护我们的资源安全。比如我们发邮件，里面的文本文件不能被随意删除，脱离我们的权限了，所以区分了权限问题，不要互相干扰。

内核空间：
#代表了我们的Win系统，Linux系统，硬盘里面的数据我们想要去读，可以通过代码，C#程序是不能直接读的，需要SDK，转换代码到我们系统能明白的01指令，然后去操作去读，
#具体的01指令操作就是内核去读。所以先由内核拷贝资源，然后递给用户空间，用户空间操作后发送资源回来，内核空间继续操作。
#不经过允许，内核空间不能随意操作我们的资源，不能随意删除东西的。所以我们需要主动把资源给过来，然后才能进行删除。
#所以读写拷贝很复杂。
```

- 没有零拷贝之前，我们要做三次拷贝（内核-用户-内核）
- 目的：保证我们的资源安全，搞了三次拷贝

![image-20230628111131834](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230628111131834.png)

```sh
# 太复杂了，所以出来了零拷贝
零拷贝就类似于我们的共享文件夹，快捷方式文件夹，Linux系统实现的。
```

---

### 消费者：

- 推送：是kafka主动去推数据，如果遇到高并发的时候，可能消费端还没有把之前的数据处理完，然后强推了大量的数据过来，有可能造成我们消费端的挂机
- 拉取：是消费端主动的去拉取，可能存在数据延迟消费，不会造成我们消费端的宕机，同样的存在一个微循环，不停的拉取数据

```sh
# Kafka是实现了拉的方式，RabbitMQ在这方面要做的比Kafka好一些了
```

为了知道我们消费到了哪里，所以，需要知道我们消费的消息位置，最好的方式就是：<u>偏移量</u>

```C#
0.9版本-> Zookeeper
0.9版本之后->自维护(topic: __consumer_offsets)
```

```C#
//Latest = 0,表示消费者消费启动之后的数据。。。启动之前的数据消费不到
//Earliest = 1,每次都从头开始消费没有消费过的数据
//Error = 2   会报错，不用，直接不了解
AutoOffsetReset = AutoOffsetReset.Earliest,
```

```sh
#演示：
几个分区，几个轮询，单个分区没有轮询
#消费端：组和组之间是广播模式，组内是根据分区数量
一定一定要注意，如果要消费者做负载均衡--则分区数量最好和同一个组的消费者数量一致
topic的数量=broker的数量
broker数量=分区数量
分区数量=一个组内的消费者数量
```

---

### 底层异常情况处理方案：

![image-20230629212239833](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230629212239833.png)

```sh
# 如果同步数据时候，同步速率不同，这时候主节点宕机了，那么消费者应该怎么去取数据
取最低
```

---

#### 高级消费循环：

```C#
//代表数据超过了6000没有处理完业务，则把数据给其他消费端
// 一定要注意。。SessionTimeoutMs值一定要小于MaxPollIntervalMs
SessionTimeoutMs = 6000,
MaxPollIntervalMs = 10000,
```

----

#### 存储和索引查找机制：

![image-20230629215050313](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230629215050313.png)

```sh
## 一个分区中有多个Segment，每个Segment对应2个发文件
## 文件会合并，且重组
```

![image-20230629215142988](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230629215142988.png)

![image-20230629215155049](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230629215155049.png)

```sh
## 每个文件的名称代表它的偏移量，Index是索引文件，二分查找算法获取！
```

![image-20230629215447054](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230629215447054.png)

```sh
## 加速搜索   偏移量——主键——消息
```

![image-20230629215630591](D:\Z_Learn\jyzl\讲义知识整理\TyporaImages\image-20230629215630591.png)

```sh
## 日志压缩策略
```

---

客户端记录偏移量方式：

```C#
// 我指定读取那个一个,哪一个偏移量
//如果没有使用消息的唯一主键来解决重新重复
// 当防止重复消费的时候，需要把消费的偏移量记录
// 消费的时候，把消费的偏移量记录在数据库里面，然后当消费端宕机之后，重新启动的时候，从数据库里面拿到偏移量，使用这个方法指定消费
// 一定要注意，这样的话，当消费者增加的时候，不会负载均衡
// 开发的时候规划好的  
	
// 我指定读取那个一个,,哪一个偏移量
consumer.Assign(topics.Select(topic => new TopicPartitionOffset(topic, 1, 6)).ToList());
```

---

### Kafka的一些问题：

**1、消费数据是先消费Leader中数据还是副本中数据**

Leader

**2、副本和leader就是两份一模一样的数据，消费后，两份数据都清空嘛，什么时候清空**

保留7天，kafka可以配置。。默认7天，消息积压有处理。。 

**3、4个节点，一个topic，4个分区。。。 应该？个副本，**

能保证只要剩下都能正常进行业务不丢失。。。 肯定是4个，但是慎重，不能太多，有压力

**4、在数据完整性方面，RabbitMq会比Kafka好吗？**

必然：RabbitMq要好，和开发语言还有网络有关系，但只要是人写的代码肯定多多稍稍有问题。win系统，蓝屏，死机，

**5、在同一个Topic下，可以根据Key来区分消息类型吗？**

不能，消费端只关心topic和偏移量--其余不关心。。

---

**6、集群才有分区概念？单机没有？**

都有，但是单机搞分区无意义。。反而降低性能，不要忘了初衷。。。

**7、加入了消息队列组件 是不是所有的业务请求都走消息队列 还是特定的写入场景才需要队列呢**

比如：微服务里面事件企业总线。mq..  解耦，削峰，必选mq..

**8、Kalfa数据落盘，leader 和 follower 可以共用吗？ 还是必须，各备份各的**

各是各的。。。

**9、队列，TOPIC，分区 不能由生产者创建吧？不能源码创建？必须每次从管理页面手动创建？**

可以，但不要这么做，最好提前规划好。没有讲，自己去看文档，看代码就好

**10、卡夫卡 是不是不能像rabbitmq消费了就提交，删数据。只能通过自身 日志压缩 删除。**

保留7天-- 可配置

---

**11、通过UI可以把分区数量动态增加或减少吗。**

可以，直接编辑里操作就好了，实际情况，先扩容broker，然后在增加分区

**12、多少吞吐量以上用kafka，就是多少之下用rabbitmq就可以了（大约）**

kafka--重，服务器--集群+分片 

**13、kafka和rabbitmq的使用场景区别**

结合实际业务，然后根据优点和缺点来选型，kafka重--- 

rabbitmq轻--丢数据的概率比kafka小，集群--镜像不能负载均衡--》最终处理的还是一个

