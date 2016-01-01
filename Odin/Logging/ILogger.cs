namespace Odin.Logging
{
    /// <summary>
    /// Interface for logging services in Odin.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs debug-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// Logs info-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Info(string format, params object[] args);

        /// <summary>
        /// Logs warning-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Warning(string format, params object[] args);

        /// <summary>
        /// Logs error-level messages.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Error(string format, params object[] args);
    }
}
