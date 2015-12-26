namespace Odin.Configuration
{
    public abstract class HelpGenerator
    {
        public abstract string Emit(Command command, string actionName = "");

    }
}