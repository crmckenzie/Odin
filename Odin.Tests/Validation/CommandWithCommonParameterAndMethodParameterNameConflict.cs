using Odin.Attributes;

namespace Odin.Tests.Validation
{
    public class CommandWithCommonParameterAndMethodParameterNameConflict : Command
    {
        [Parameter]
        public string Param1 { get; set; }

        [Action]
        public void Action1([Alias("p")] string param1)
        {

        }
    }
}