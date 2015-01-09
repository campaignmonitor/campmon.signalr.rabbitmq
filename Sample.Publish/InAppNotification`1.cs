using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace Sample.Publish
{
    public class InAppNotification<T> : IDisposable
    {
        private readonly HubConnection connection;
        private readonly IHubProxy proxy;

        public InAppNotification(string connectionUrl)
        {
            connection = new HubConnection(connectionUrl);
            var type = typeof (T);
            if (!type.IsInterface || !type.Name.StartsWith("I"))
            {
                throw new ArgumentException("Type must be an interface, and use the default naming convention");
            }
            proxy = connection.CreateHubProxy(typeof(T).Name.Remove(0, 1));
        }

        public async Task Push(Expression<Action<T>> function)
        {
            var body = new UnwrapMethodCall(function.Body);
            await connection.Start();
            await proxy.Invoke(body.MethodName, body.Arguments);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                connection.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
