using Odin.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odin.Tests.Parsing;

namespace Odin.Tests.Lib
{
    [Description(@"This is a demo of the Odin-Commands NuGet package.
You can use this package to easily create command line applications that automatically route command-line arguments to the correct command based on customizable conventions.")]
    public class HelpWriterTestCommand : Command
    {
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
        [Description("Enumerated values should be listed before default values.")]
        public void EnumAction(Numbers input = Numbers.One)
        {
            
        }
    }
}
