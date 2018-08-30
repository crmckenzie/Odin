using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Attributes
{
    /// <summary>
    /// Identifies aliases for <see cref="Command">Commands</see>, 
    /// <see cref="ActionAttribute">Actions</see>, or <see cref="ActionParameter">Parameters.</see>
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Parameter | 
        AttributeTargets.GenericParameter | 
        AttributeTargets.Class | 
        AttributeTargets.Property |
        AttributeTargets.Method, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Gets the list of aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        /// Identifies aliases for <see cref="Command">Commands</see>
        /// </summary>
        /// <param name="aliases"></param>
        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }


    }
}
