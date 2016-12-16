using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            //fanout类型的exchange类似于广播，可以启动多个Consumer实例接收所有的消息，没有过滤机制

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var message = "hello world";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs", "", null, body);

                Console.WriteLine("Sent " + message);
            }

            Console.ReadLine();
        }
    }
}
