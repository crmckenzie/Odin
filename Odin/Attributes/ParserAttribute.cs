using System;
using Odin.Parsing;

namespace Odin.Attributes
{
    /// <summary>
    /// Used to specify that a <see cref="ParameterValue">Parameter</see> is parsed using a
    /// custom parser.
    /// </summary>
    /// <remarks>
    /// Parser types are expected to implement <see cref="IParser"/> and to provide a constructor
    /// that accepts a <see cref="ParameterValue"/> instance.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
    public class ParserAttribute : System.Attribute
    {
        /// <summary>
        /// Gets the  type of parser to be used.
        /// </summary>
        public Type ParserType { get; }

        public ParserAttribute(System.Type parserType)
        {
            this.ParserType = parserType;
        }
    }
}