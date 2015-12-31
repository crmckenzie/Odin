using System;

namespace Odin.Attributes
{
    /// <summary>
    /// Identifies a method on a <see cref="Command"></see> class as an executable action. />
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : System.Attribute
    {
        public bool IsDefault { get; set; }
    }
}