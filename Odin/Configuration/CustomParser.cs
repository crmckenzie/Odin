using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Configuration
{
    public abstract class CustomParser
    {
        public abstract ParseResult Parse(string[] tokens, int i);
    }
}
