using System;

namespace Odin
{
    public class DefaultLogger : Logger
    {
        public override void Debug(string format, params object[] args)
        {
            System.Diagnostics.Debug.Write(string.Format(format, args));
        }

        public override void Info(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public override void Warning(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public override void Error(string format, params object[] args)
        {
            Console.Error.Write(format, args);
        }
    }
}