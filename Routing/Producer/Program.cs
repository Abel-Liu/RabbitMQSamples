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
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("direct_logs", "direct");

                string[] levels = new string[] { "info", "warning", "error" };
                int i = 0;

                while (i < 10)
                {
                    var level = levels[i % 3];

                    var message = level + " at " + DateTime.Now.ToString("mm:ss.fff");
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("direct_logs", level, null, body);
                    Console.WriteLine("Sent " + message);

                    i++;
                    System.Threading.Thread.Sleep(100);
                }
            }

            Console.ReadLine();
        }
    }
}
