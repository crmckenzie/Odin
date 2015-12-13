using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
    public class AliasAttribute : Attribute
    {
        public IReadOnlyCollection<string> Aliases { get; }

        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases.ToList().AsReadOnly();
        }
    }
}
