using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CampMon.SignalR.RabbitMQ;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Owin.Hosting;
using Owin;

namespace Sample.WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = GetPort(args);
            var url = "http://localhost:" + port.ToString();
            
            using (WebApp.Start(url))
            {
                using (var connection = new HubConnection(url))
                {
                    var proxy = connection.CreateHubProxy("DiagnosticHub");
                    proxy.On("Log", (obj) => Console.WriteLine(obj));
                    connection.Start(new LongPollingTransport()).Wait();
                    proxy.Invoke("Log", String.Format("Proxy initialising on port: {0}",port)).Wait();
                    Console.WriteLine("Web server started");
                    Console.ReadLine();
                }
            }
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

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HubConfiguration
            {
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true,
            };
            
            config.Resolver.UseRabbitMQ("rabbitmq://192.168.59.103", "guest", "guest");
            
            app.MapSignalR(config);
        }

    }

}
