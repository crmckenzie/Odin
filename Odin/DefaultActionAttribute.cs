using System;

namespace Odin
{
    public class DefaultActionAttribute : Attribute
    {
        public string MethodName { get; }

        public DefaultActionAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}