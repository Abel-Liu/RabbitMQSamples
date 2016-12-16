using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task = new Task();
            task.Start();

            Console.ReadLine();
            task.Dispose();
        }
    }
}
