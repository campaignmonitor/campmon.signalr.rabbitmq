using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hub
{
    public interface IDiagnosticHub
    {
        void Log(object log);
    }

    public enum LoggingLevel
    {
        Info,
        Debug,
        Warn,
        Error,
    }


}
