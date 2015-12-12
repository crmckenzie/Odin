namespace Odin.Logging
{
    public abstract class Logger
    {
        public abstract void Debug(string format, params object[] args);

        public abstract void Info(string format, params object[] args);

        public abstract void Warning(string format, params object[] args);

        public abstract void Error(string format, params object[] args);
    }
}
