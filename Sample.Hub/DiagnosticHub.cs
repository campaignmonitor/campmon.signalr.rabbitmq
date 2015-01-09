using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Sample.Hub
{
    public class DiagnosticHub : Hub<IDiagnosticClient>, IDiagnosticHub
    {
        public void Log(object log)
        {
            Clients.All.Log(log);
        }
    }
}
