using Odin.Attributes;

namespace Odin.Tests.Validation
{
    public class CommandWithSharedParameterAliasDuplication : Command
    {
        [Parameter]
        [Alias("p")]
        public string Param1 { get; set; }

        [Parameter]
        [Alias("p")]
        public string Param2 { get; set; }

        [Alias]
        public void DoStuff()
        {
            
        }
    }
}