using System;
using Odin.Parsing;

namespace Odin.Attributes
{
    /// <summary>
    /// Used to specify that a <see cref="MethodParameter">Parameter</see> is parsed using a
    /// custom parser.
    /// </summary>
    /// <remarks>
    /// Parser types are expected to implement <see cref="IParser"/> and to provide a constructor
    /// that accepts a <see cref="MethodParameter"/> instance.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter 
        | AttributeTargets.GenericParameter 
        | AttributeTargets.Property)]
    public class ParserAttribute : System.Attribute
    {
        /// <summary>
        /// Gets the  type of parser to be used.
        /// </summary>
        public Type ParserType { get; }

        /// <summary>
        /// Used to specify that a <see cref="MethodParameter">Parameter</see> is parsed using a
        /// custom parser.
        /// </summary>
        /// <remarks>
        /// Parser types are expected to implement <see cref="IParser"/> and to provide a constructor
        /// that accepts a <see cref="MethodParameter"/> instance.
        /// </remarks>
        public ParserAttribute(System.Type parserType)
        {
            this.ParserType = parserType;
        }
    }
}