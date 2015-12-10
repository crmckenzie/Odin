using System;

namespace Odin
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ActionAttribute : System.Attribute
    {
        public bool IsDefault { get; set; }
    }
}