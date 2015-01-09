using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Bootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 8000;
            var workers = new Queue<Process>();

            var master = StartNew(port++);
            workers.Enqueue(StartNew(port++));
            workers.Enqueue(StartNew(port++));

            var command = Console.ReadLine();
            while (!String.Equals(command, "quit", StringComparison.InvariantCultureIgnoreCase))
            {
                if (String.Equals(command, "up", StringComparison.InvariantCultureIgnoreCase))
                {
                    workers.Enqueue(StartNew(port++));
                }

                if (String.Equals(command, "down", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (workers.Count > 0)
                    {
                        var process = workers.Dequeue();

                        Console.WriteLine("dropping worker {0}", process.Id);
                        process.CloseMainWindow();
                        process.Dispose();
                    }
                    else
                    {
                        Console.WriteLine("quit to drop the final worker");
                    }

                }
                command = Console.ReadLine();
            }
            
            while (workers.Count > 0)
            {
                var process = workers.Dequeue();
                process.CloseMainWindow();
                process.Dispose();
            }
            master.CloseMainWindow();
            master.Dispose();
        }

        private static Process StartNew(int port)
        {
            Console.WriteLine("Starting worker on port: {0}", port);

            var args = new ProcessStartInfo(typeof (Sample.WebServer.Startup).Assembly.Location, port.ToString())
            {
                Verb = "runas"
            };
            var process = Process.Start(args);

            Console.WriteLine("worker {1} started on port: {0}", port, process.Id);

            return process;
        }
    }
}
