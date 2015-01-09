using System;
using Microsoft.AspNet.SignalR.Messaging;

namespace CampMon.SignalR.RabbitMQ
{
    public class RabbitScaleoutConfiguration : ScaleoutConfiguration
    {
        public string Host { get; private set; }
        public string VirtualHost { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Exchange { get; private set; }

        public RabbitScaleoutConfiguration(string host, string virtualHost, string exchange, string username, string password)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (username == null) throw new ArgumentNullException("username");
            if (password == null) throw new ArgumentNullException("password");

            Host = host;
            VirtualHost = virtualHost;
            Username = username;
            Password = password;
            Exchange = exchange;
        }
    }
}
