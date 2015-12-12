using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odin.Attributes;
using Odin.Demo;
using Odin.Logging;

namespace Odin.Tests
{
    [Description("Provides a component of testability for subcommands.")]
    public class SubCommand : Command
    {

        public SubCommand(Logger logger) : base(logger)
        {
            base.RegisterSubCommand(new KatasCommand());
        }

        public SubCommand() : this(new DefaultLogger())
        {
            
        }

        [Action(IsDefault = true)]
        public virtual void DoSomething()
        {
            this.Logger.Info("Do something!");
        }
    }
}
