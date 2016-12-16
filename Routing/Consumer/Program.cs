using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("direct_logs", "direct");
                var queueName = channel.QueueDeclare().QueueName;

                //不同实例订阅若干不同routing，可实现消息过滤
                channel.QueueBind(queueName, "direct_logs", "info");
                channel.QueueBind(queueName, "direct_logs", "warning");
                channel.QueueBind(queueName, "direct_logs", "error");

                Console.WriteLine("Waiting for messages...");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = e.RoutingKey;
                    Console.WriteLine("Received {0}: {1}", routingKey, message);
                };
                channel.BasicConsume(queueName, true, consumer);

                Console.ReadLine();
            }
        }
    }
}
