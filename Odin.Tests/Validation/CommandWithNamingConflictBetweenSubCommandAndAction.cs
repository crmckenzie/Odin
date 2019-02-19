using Odin.Attributes;
using KatasCommand = Odin.Tests.Samples.Demo.KatasCommand;

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