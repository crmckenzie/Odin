using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin
{
    public delegate void LoggingEvent(string text);

    public class Logger
    {
        public event LoggingEvent OnInfo;
        public event LoggingEvent OnWarning;
        public event LoggingEvent OnError;

        public virtual void Info(string format, params object[] args)
        {
            OnInfo?.Invoke(string.Format(format, args));
        }

        public virtual void Warning(string format, params object[] args)
        {
            OnWarning?.Invoke(string.Format(format, args));
        }
        public virtual void Error(string format, params object[] args)
        {
            OnError?.Invoke(string.Format(format, args));
        }
    }
}
