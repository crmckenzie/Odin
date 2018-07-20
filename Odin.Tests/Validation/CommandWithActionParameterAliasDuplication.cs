using Odin.Attributes;

namespace Odin.Tests.Validation
{
    public class CommandWithActionParameterAliasDuplication : Command
    {
        [Action]
        public void Action1([Alias("p")] string param1, [Alias("p")] string param2)
        {
            
        }
    }
}