using System.ComponentModel;
using Odin.Attributes;
using Odin.Tests.Parsing;

namespace Odin.Tests.Help
{
    [Description(@"This is a demo of the Odin-Commands NuGet package.
You can use this package to easily create command line applications that automatically route command-line arguments to the correct command based on customizable conventions.")]
    public class HelpWriterTestCommand : Command
    {
        [Parameter]
        [Alias("c")]
        [Description(@"Shared parameters are displayed after all of the actions")]
        public string Shared { get; set; }

        [Action(IsDefault = true)]
        [Alias("default")]
        [Description(@"Use the ActionAttribute to indicate which methods should be mapped to command line arguments. Set IsDefault=True to mark an action as the default. Only one action per command can be marked default.
Use the AliasAttribute to indicate an alias for the action.")]
        public void DefaultAction(
            [Alias("p", "p1")]
            [Description("Use the AliasAttribute to indicate one or more aliases for the parameter.")]
            int param1, 
            [Alias("r", "p2")]
            [Description(@"Default values are displayed first, followed by aliases. Each parameter can have its own DescriptionAttribute.")]
            string param2 = "42")
        {
            
        }

        [Action]
        [Alias("enum")]
        [Description("Enumerated parameters should be listed before default value.")]
        public void EnumAction(Numbers input = Numbers.One)
        {
            
        }

        [Action]
        [Description("Boolean parameters should list the negative option before default value.")]
        public void BooleanAction(bool input = false)
        {
            
        }
    }
}
