using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Parsing;

namespace Odin
{
    /// <summary>
    /// Represents an action parameter and its value.
    /// </summary>
    public class ParameterValue
    {
        private bool _isSet;

        private object _value;

        public ParameterValue(MethodInvocation methodInvocation, ParameterInfo parameterInfo)
        {
            MethodInvocation = methodInvocation;
            ParameterInfo = parameterInfo;

            if (ParameterType == typeof (bool))
                Value = false;

            if (ParameterType.IsNullableType())
                Value = null;

            if (ParameterInfo.IsOptional)
                Value = Type.Missing;
        }

        private MethodInvocation MethodInvocation { get; }

        /// <summary>
        /// Gets the conventions used in the command tree.
        /// </summary>
        public IConventions Conventions => MethodInvocation.Conventions;

        /// <summary>
        /// Gets the ParameterInfo wrapped by the ParameterValue.
        /// </summary>
        public ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Gets the Type of the parameter.
        /// </summary>
        public Type ParameterType => ParameterInfo.ParameterType;

        /// <summary>
        /// Gets the position of the parameter.
        /// </summary>
        public int Position => ParameterInfo.Position;

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isSet = true;
            }
        }


        /// <summary>
        /// Gets the conventional name of the parameter.
        /// </summary>
        public string Name => ParameterInfo.Name;

        /// <summary>
        /// Gets the conventional long option name for the parameter.
        /// </summary>
        public string LongOptionName => Conventions.GetLongOptionName(Name);

        public string[] Aliases
        {
            get
            {
                var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
                if (attr == null)
                    return new string[] {};

                return attr.Aliases.Select(a => Conventions.GetShortOptionName(a)).ToArray();
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

            if (IsBoolean() && Conventions.IsNegatedLongOptionName(this.Name, token))
                return true;

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
            return ParameterInfo.GetCustomAttribute<ParserAttribute>() != null;
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
    }
}