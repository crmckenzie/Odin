using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Parsing;

namespace Odin.Configuration
{
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

        public override bool IsMatchingParameter(ParameterValue parameterMap, string arg)
        {
            return parameterMap.LongOptionName == arg;
        }

        public override IParser CreateParser(ParameterValue parameter)
        {
            return new DefaultHyphenCaseParser(parameter);
        }

        public static bool IsOptionIdentifier(string value)
        {
            return value.StartsWith("--") ||value.StartsWith("-");
        }
    }
}