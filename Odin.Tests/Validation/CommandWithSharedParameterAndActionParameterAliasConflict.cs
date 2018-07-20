using Odin.Attributes;

namespace Odin.Tests.Validation
{
    public class CommandWithSharedParameterAndActionParameterAliasConflict : Command
    {
        [Parameter]
        [Alias("p")]
        public string SharedParameter { get; set; }

        [Action]
        public void Action1([Alias("p")] string param1)
        {

        }
    }
}