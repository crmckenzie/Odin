using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [Description("Provides a component of testability for subcommands.")]
    public class SubCommandCommand : Command
    {

        public SubCommandCommand(Logger logger) : base(logger)
        {
        }

        public SubCommandCommand() : base()
        {
            
        }

        [Action(IsDefault = true)]
        public virtual void DoSomething()
        {
            
        }
    }
}
