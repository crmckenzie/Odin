using Odin.Attributes;

namespace Odin.Tests.Validation
{
    public class CommandWithMultipleDefaultActions : Command
    {
        [Action(IsDefault = true)]
        public void Action1()
        {
        }

        [Action(IsDefault = true)]
        public void Action2()
        {
        }
    }
}