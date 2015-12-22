using System;

namespace Odin.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
    public class ParserAttribute : System.Attribute
    {
        public Type ParserType { get; }

        public ParserAttribute(System.Type parserType)
        {
            this.ParserType = parserType;
        }
    }
}