using System;

namespace Odin.Logging
{
    /// <summary>
    /// Logger implementation that writes messages to stdOut and stdError.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Debug(string format, params object[] args)
        {
            System.Diagnostics.Debug.Write(string.Format(format, args));
        }

        public void Info(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public void Warning(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public void Error(string format, params object[] args)
        {
            Console.Error.Write(format, args);
        }
    }
}