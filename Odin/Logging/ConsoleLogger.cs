using System;

namespace Odin.Logging
{
    /// <summary>
    /// Logger implementation that writes messages to stdOut and stdError.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs debug-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Debug(string format, params object[] args)
        {
            System.Diagnostics.Debug.Write(string.Format(format, args));
        }

        /// <summary>
        /// Logs info-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Info(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        /// <summary>
        /// Logs warning-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Warning(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        /// <summary>
        /// Logs error-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Error(string format, params object[] args)
        {
            Console.Error.Write(format, args);
        }
    }
}