using Odin.Attributes;
using Odin.Demo;

namespace Odin.Tests.Validation
{
    public class CommandWithNamingConflictBetweenSubCommandAndAction : Command
    {
        [Action]
        public void Katas()
        {
        }

        public CommandWithNamingConflictBetweenSubCommandAndAction()
        {
            this.RegisterSubCommand(new KatasCommand());
        }
    }
}