using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hub
{
    public interface IDiagnosticClient
    {
        void Log(object log);
    }
}
