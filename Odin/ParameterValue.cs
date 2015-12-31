using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Parsing;

namespace Odin
{
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

        public IConventions Conventions => MethodInvocation.Conventions;

        public ParameterInfo ParameterInfo { get; }
        public Type ParameterType => ParameterInfo.ParameterType;

        public int Position => ParameterInfo.Position;

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isSet = true;
            }
        }

        public string Name => ParameterInfo.Name;

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

        public bool IsValueSet()
        {
            return _isSet;
        }

        public bool IsBoolean()
        {
            return ParameterType.IsBoolean()       ;
        }

        public bool HasCustomParser()
        {
            return ParameterInfo.GetCustomAttribute<ParserAttribute>() != null;
        }

        public bool IsNegatedBy(string token)
        {
            if (!IsBoolean())
                return false;

            return Conventions.IsNegatedLongOptionName(Name, token);
        }
    }
}