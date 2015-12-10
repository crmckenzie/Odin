namespace Odin.Demo
{
    public class RootCommand : Command
    {

        public RootCommand() : this(new FizzBuzzCommand())
        {
        }

        public RootCommand(Logger logger) : this(logger, new FizzBuzzCommand())
        {
        }

        public RootCommand(FizzBuzzCommand fizzbuzz) : this(null, fizzbuzz)
        {
        }

        public RootCommand(Logger logger, FizzBuzzCommand fizzbuzz) : base(logger)
        {
            base.RegisterSubCommand(fizzbuzz);
        }
    }
}