using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Sample.Hub;

namespace Sample.Publish
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = GetPort(args);

            Console.WriteLine("Hit enter when servers have started..");
            Console.ReadLine();

            Help();

            using (var push = new InAppNotification<IDiagnosticHub>("http://localhost:" + port.ToString()))
            {
                var command = Console.ReadLine();
                while (!String.Equals(command, "quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!String.IsNullOrWhiteSpace(command))
                    {
                        var parts = command.Split(new[] {':'}, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            var level = parts.First();
                            var message = parts.Skip(1).First();
                            LoggingLevel loggingLevel;
                            if (Enum.TryParse(level, out loggingLevel))
                            {
                                push.Push(p => p.Log(new { loggingLevel, message })).Wait();
                            }
                            else
                            {
                                Help();
                            }
                        }
                        else
                        {
                            push.Push(p => p.Log(command)).Wait();
                        }


                    }
                    command = Console.ReadLine();
                }
            }
        }

        private static void Help()
        {
            Console.WriteLine("Enter a message eg: debug:Hello world or quit");
        }

        public static int GetPort(string[] args)
        {
            var arg = args.FirstOrDefault();
            if (String.IsNullOrEmpty(arg))
            {
                return 8000;
            }
            int port;
            if (!int.TryParse(arg, out port))
            {
                throw new ArgumentException("must be a value port", "port");
            }
            return port;
        }
    }
}
