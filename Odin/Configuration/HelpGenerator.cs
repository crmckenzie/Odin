namespace Odin.Configuration
{
    public abstract class HelpGenerator
    {
        public abstract string GenerateHelp(Command command, string actionName = "");

    }
}