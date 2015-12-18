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

        public override string GetLongOptionName(ParameterInfo row)
        {
            return $"--{row.Name.HyphenCase()}";
        }

        public override string GetActionName(MethodInfo methodInfo)
        {
            return methodInfo.Name.HyphenCase();
        }

        public override string GetShortOptionName(string rawAlias)
        {
            return $"-{rawAlias}";
        }

        public override bool IsIdentifiedBy(ParameterValue parameterMap, string arg)
        {
            return parameterMap.LongOptionName == arg;
        }

        public override IValueParser GetParser(ParameterValue parameter)
        {
            return new HyphenCaseValueParser(this, parameter);
        }
    }
}