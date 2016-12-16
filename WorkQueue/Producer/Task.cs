using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Threading;

namespace Producer
{
    public class Task : IDisposable
    {
        List<Thread> sendThreads = new List<Thread>();

        public void Start()
        {
            Thread t1 = new Thread(new ParameterizedThreadStart(Send));
            t1.Start("one,500");

            Thread t2 = new Thread(new ParameterizedThreadStart(Send));
            t2.Start("two,1000");

            Thread t3 = new Thread(new ParameterizedThreadStart(Send));
            t3.Start("three,300");

            sendThreads.AddRange(new Thread[] { t1, t2, t3 });
        }

        private void Send(object obj)
        {
            var args = ((string)obj).Split(',');
            var name = args[0];
            var milli = int.Parse(args[1]);

            var fac = new ConnectionFactory() { HostName = "localhost" };
            using (var conn = fac.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    int i = 1;
                    while (i <= 5)
                    {
                        var msg = name + " " + i.ToString();
                        var body = Encoding.UTF8.GetBytes(msg);

                        channel.BasicPublish(string.Empty, "hello", properties, body);
                        Console.WriteLine("Sent: " + msg);

                        i++;
                        Thread.Sleep(milli);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var t in sendThreads)
            {
                try
                {
                    t.Abort();
                }
                catch { }
            }
        }
    }
}
