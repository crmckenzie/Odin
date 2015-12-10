using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [Description("Provides a component of testability for subcommands.")]
    [DefaultAction("DoSomething")]
    public class SubCommandCommandRoute : CommandRoute
    {
        public SubCommandCommandRoute(Logger logger) : base()
        {
            this.Logger = logger;
        }

        public SubCommandCommandRoute() : base()
        {
            
        }

        [Action]
        public virtual void DoSomething()
        {
            
        }
    }
}
