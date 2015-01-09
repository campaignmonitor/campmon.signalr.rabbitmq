using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampMon.SignalR.RabbitMQ
{
    public class RabbitMessageBus : ScaleoutMessageBus
    {
        private readonly IModel channel;
        private readonly RabbitScaleoutConfiguration configuration;
        private readonly Task worker;
        private static readonly object locker = new object();

        public RabbitMessageBus(IDependencyResolver resolver, RabbitScaleoutConfiguration configuration)
            : base(resolver, configuration)
        {
            if(configuration == null) throw new ArgumentNullException("configuration");

            this.configuration = configuration;
            
            var factory = new ConnectionFactory
            {
                UserName = configuration.Username,
                Password = configuration.Password,
                Endpoint = new AmqpTcpEndpoint(new Uri(configuration.Host)),
            };

            if (configuration.VirtualHost != null)
            {
                factory.VirtualHost = configuration.VirtualHost;
            }

            var connection = factory.CreateConnection();

            try
            {
                channel = connection.CreateModel();
            }
            catch (Exception)
            {
                connection.Dispose();
                throw;
            }

            worker = Task.Run(() =>
            {
                Open(0);

                channel.ExchangeDeclare(configuration.Exchange, "fanout");
                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queueName, configuration.Exchange, "");

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);

                while (true)
                {
                    var args = consumer.Queue.Dequeue();
                    var message = ScaleoutMessage.FromBytes(args.Body);

                    lock (locker)
                    {
                        OnReceived(0, args.DeliveryTag, message);
                    }
                }
            });
        }

        protected override Task Send(int streamIndex, IList<Message> messages)
        {
            return Task.Run(() => channel.BasicPublish(configuration.Exchange, "", null, new ScaleoutMessage(messages).ToBytes()));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (channel != null)
                {
                    channel.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
