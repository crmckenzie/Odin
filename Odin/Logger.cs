using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin
{
    public delegate void LoggingEvent(string text);

    public abstract class Logger
    {
        public abstract void Debug(string format, params object[] args);

        public abstract void Info(string format, params object[] args);

        public abstract void Warning(string format, params object[] args);

        public abstract void Error(string format, params object[] args);
    }
}
