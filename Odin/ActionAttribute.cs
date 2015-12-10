using System;

namespace Odin
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ActionAttribute : System.Attribute
    {
        public bool IsDefault { get; set; }
    }
}