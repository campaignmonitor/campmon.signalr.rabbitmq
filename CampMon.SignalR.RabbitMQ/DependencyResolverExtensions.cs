using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using System;

namespace CampMon.SignalR.RabbitMQ
{
    public static class DependencyResolverExtensions
    {
        public static IDependencyResolver UseRabbitMQ(this IDependencyResolver source, string host, string username, string password, string virtualHost = null, string exchange = "signalr-backplane")
        {
            if (source == null) throw new ArgumentNullException("source");
            if (host == null) throw new ArgumentNullException("host");
            if (username == null) throw new ArgumentNullException("username");
            if (password == null) throw new ArgumentNullException("password");

            var configuration = new RabbitScaleoutConfiguration(host, virtualHost, exchange, username, password);
            var messageBus = new Lazy<RabbitMessageBus>(() => new RabbitMessageBus(source, configuration));
            
            source.Register(typeof(IMessageBus), () => messageBus.Value);

            return source;
        }
    }
}
