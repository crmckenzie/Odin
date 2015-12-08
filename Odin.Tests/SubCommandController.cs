using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Tests
{
    [DefaultAction("DoSomething")]
    public class SubCommandController : Controller
    {
        public virtual void DoSomething()
        {
            
        }
    }
}
