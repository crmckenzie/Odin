using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [Description("Provides a component of testability for subcommands.")]
    public class SubCommand : Command
    {

        public SubCommand(Logger logger) : base(logger)
        {
        }

        public SubCommand() : base()
        {
            
        }

        [Action(IsDefault = true)]
        public virtual void DoSomething()
        {
            
        }
    }
}
