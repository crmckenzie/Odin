namespace Odin.Demo
{
    public class RootCommand : Command
    {

        public RootCommand() : this(new KatasCommand())
        {
        }

        public RootCommand(Logger logger) : this(logger, new KatasCommand())
        {
        }

        public RootCommand(KatasCommand fizzbuzz) : this(null, fizzbuzz)
        {
        }

        public RootCommand(Logger logger, KatasCommand fizzbuzz) : base(logger)
        {
            base.RegisterSubCommand(fizzbuzz);
        }
    }
}