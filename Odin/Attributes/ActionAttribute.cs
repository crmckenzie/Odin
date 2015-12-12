using System;

namespace Odin.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : System.Attribute
    {
        public bool IsDefault { get; set; }
    }
}