using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Exceptions;
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

        public Conventions Conventions => MethodInvocation.Conventions;

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

        public string GetDescription()
        {
            var attr = ParameterInfo.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
                return attr.Description;
            return "";
        }

        public bool IsValueSet()
        {
            return _isSet;
        }

        internal bool IsBoolean()
        {
            return ParameterType.IsBoolean()
                ;
        }

        public bool IsIdentifiedBy(string token)
        {
            if (Conventions.IsMatchingParameter(this, token))
                return true;
            return HasAlias(token);
        }

        private bool MatchesAlias(AliasAttribute aliasAttribute, string arg)
        {
            if (aliasAttribute == null)
                return false;

            var aliases = aliasAttribute.Aliases.Select(Conventions.GetShortOptionName);
            return aliases.Contains(arg);
        }

        private bool HasAlias(string arg)
        {
            var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
            return MatchesAlias(attr, arg);
        }

        public bool HasAliases()
        {
            var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
            if (attr == null)
                return false;

            return attr.Aliases.Any();
        }

        public string[] GetAliases()
        {
            var attr = ParameterInfo.GetCustomAttribute<AliasAttribute>();
            if (attr == null)
                return new string[] {};

            return attr.Aliases.Select(a => Conventions.GetShortOptionName(a)).ToArray();
        }

        public bool HasCustomParser()
        {
            return ParameterInfo.GetCustomAttribute<ParserAttribute>() != null;
        }

        public bool IsNegatedBy(string token)
        {
            if (!IsBoolean())
                return false;

            return Conventions.IsNegatedLongOptionName(this.Name, token);
        }
    }
}