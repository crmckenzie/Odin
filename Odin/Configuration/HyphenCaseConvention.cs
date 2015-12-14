using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Odin.Attributes;

namespace Odin.Configuration
{
    public static class StringExtensions
    {
        public static string HyphenCase(this string input)
        {
            var result = Regex.Replace(input, ".[A-Z]", m => m.Value[0] + "-" + m.Value[1]).ToLower();
            return result;
        }
    }

    public class HyphenCaseConvention : Conventions
    {
        public override string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            var hyphenCased = name.Replace("Command", "").HyphenCase();
            return hyphenCased;
        }

        public override string GetArgumentName(ParameterInfo row)
        {
            return $"--{row.Name.HyphenCase()}";
        }

        public override bool IsArgumentIdentifier(string value)
        {
            return value.StartsWith("--");
        }

        public override string GetActionName(MethodInfo methodInfo)
        {
            return methodInfo.Name.HyphenCase();
        }

        public override bool MatchesAlias(AliasAttribute aliasAttribute, string arg)
        {
            if (aliasAttribute == null)
                return false;

            var aliases = aliasAttribute.Aliases.Select(GetFormattedAlias);
            return aliases.Contains(arg);
        }

        public override string GetFormattedAlias(string rawAlias)
        {
            return $"-{rawAlias}";
        }

        public override bool IsIdentifiedBy(ParameterMap parameterMap, string arg)
        {
            return parameterMap.Switch == arg;
        }

        public override int SetValue(ParameterValue parameter, int i)
        {
            var arg = parameter.Args[i];
            if (parameter.IsBooleanSwitch())
            {
                parameter.Value = true;
            }
            else if (parameter.NextArgIsIdentifier(i))
            {
                return 1;
            }
            else if (parameter.IsIdentifiedBy(arg) && parameter.HasNextValue(i))
            {
                var value = parameter.Args[i + 1];
                parameter.Value = parameter.Coerce(value);
                return 2;
            }
            else
            {
                parameter.Value = parameter.Coerce(arg);
            }

            return 1;
        }
    }
}