using System;
using System.Linq;
using Odin.Attributes;
using Odin.Conventions;
using Odin.Parsing;

namespace Odin
{
    /// <summary>
    /// Base class representing parameters passed to an Action
    /// </summary>
    public abstract class Parameter : IParser
    {
        private bool _isSet;
        private object _value;

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the default value of the parameter.
        /// </summary>
        public abstract object DefaultValue { get; }

        /// <summary>
        /// True if the parameter has a default value, otherwise false.
        /// </summary>
        public abstract bool HasDefaultValue { get; }

        /// <summary>
        /// Gets the <see cref="ParserAttribute"/> associated with this parameter.
        /// </summary>
        public abstract ParserAttribute ParserAttribute { get; }

        /// <summary>
        /// Gets the conventional name of the parameter.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the Type of the parameter.
        /// </summary>
        public abstract Type ParameterType { get; }


        /// <summary>
        /// Gets the <see cref="AliasAttribute"/> associated with this parameter.
        /// </summary>
        protected abstract AliasAttribute AliasAttribute { get; }

        /// <summary>
        /// Gets the conventions used in the command tree.
        /// </summary>
        public abstract IConventions Conventions { get;  }


        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public virtual object Value
        {
            get => _value;
            set
            {
                _value = value;
                _isSet = true;
            }
        }

        /// <summary>
        /// Gets the conventional long option name for the parameter.
        /// </summary>
        public string LongOptionName => Conventions.GetLongOptionName(Name);

        /// <summary>
        /// Gets the list of aliases used to identify the parameter.
        /// </summary>
        public string[] Aliases
        {
            get
            {
                if (AliasAttribute == null)
                    return new string[] { };

                return Conventions.GetShortOptionNames(AliasAttribute);
            }
        }

        /// <summary>
        /// True if the token identifies the parameter by conventional name or alias. Otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsIdentifiedBy(string token)
        {
            if (Conventions.IsMatchingParameter(this, token))
                return true;

            if (Aliases.Contains(token))
                return true;

            if (IsBoolean())
                return Conventions.IsNegatedLongOptionName(this.Name, token);

            return false;
        }

        /// <summary>
        /// True if the value has been set. Otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool IsValueSet()
        {
            return _isSet;
        }

        /// <summary>
        /// True if the parameter represents a boolean switch. Otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool IsBoolean()
        {
            return ParameterType.IsBoolean()       ;
        }

        /// <summary>
        /// True if the parameter has a custom parser. Otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool HasCustomParser()
        {
            return ParserAttribute != null;
        }

        /// <summary>
        /// True if the parameter is boolean and the token matches the expected negative form. Otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsNegatedBy(string token)
        {
            if (!IsBoolean())
                return false;

            return Conventions.IsNegatedLongOptionName(Name, token);
        }

        private IParser CreateCustomParser()
        {
            if (!ParserAttribute.ParserType.Implements<IParser>())
                throw new ArgumentOutOfRangeException($"'{ParserAttribute.ParserType.FullName}' is not an implementation of '{typeof(IParser).FullName}.'");

            var constructor = ParserAttribute.ParserType.GetConstructor(typeof(Parameter));
            if (constructor == null)
            {
                throw new TypeInitializationException(
                    $"Could not find a constructor with the signature ({typeof(Parameter).Name}).", null);
            }

            var parameters = new object[] { this };
            var instance = constructor.Invoke(parameters);
            var typedInstance = (IParser)instance;
            return typedInstance;
        }

        internal IParser CreateParser()
        {
            return HasCustomParser() ? CreateCustomParser() : Conventions.CreateParser(this);
        }

        public ParseResult Parse(string[] tokens, int tokenIndex)
        {
            var parser = CreateParser();
            return parser.Parse(tokens, tokenIndex);
        }

    }
}