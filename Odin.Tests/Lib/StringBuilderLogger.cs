using System.Text;
using Odin.Logging;

namespace Odin.Tests
{
    public class StringBuilderLogger : ILogger
    {
        public StringBuilderLogger()
        {
            this.InfoBuilder = new System.Text.StringBuilder();
            this.WarningBuilder = new System.Text.StringBuilder();
            this.ErrorBuilder = new System.Text.StringBuilder();
            this.DebugBuilder = new System.Text.StringBuilder();
        }

        public StringBuilder DebugBuilder { get; set; }

        public StringBuilder ErrorBuilder { get; }

        public StringBuilder WarningBuilder { get; }

        public StringBuilder InfoBuilder { get; }

        public void Debug(string format, params object[] args)
        {
            this.DebugBuilder.AppendFormat(format, args);
        }

        public void Info(string format, params object[] args)
        {
            this.InfoBuilder.AppendFormat(format, args);
        }
        public void Warning(string format, params object[] args)
        {
            this.WarningBuilder.AppendFormat(format, args);
        }
        public void Error(string format, params object[] args)
        {
            this.ErrorBuilder.AppendFormat(format, args);
        }

    }
}