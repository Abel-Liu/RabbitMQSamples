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
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "logs", "");

                Console.WriteLine("Waiting for receive...");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received: " + message);
                };

                channel.BasicConsume(queueName, true, consumer);

                Console.ReadLine();
            }
        }
    }
}
