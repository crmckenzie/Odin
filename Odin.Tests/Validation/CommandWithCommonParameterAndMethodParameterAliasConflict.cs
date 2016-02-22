using Odin.Attributes;

namespace Odin.Tests.Validation
{
    public class CommandWithCommonParameterAndMethodParameterAliasConflict : Command
    {
        [Parameter]
        [Alias("p")]
        public string CommonParameter { get; set; }

        [Action]
        public void Action1([Alias("p")] string param1)
        {

        }
    }
}