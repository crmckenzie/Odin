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

        public override string GetLongOptionName(string parameterName)
        {
            return $"--{parameterName.HyphenCase()}";
        }

        public override string GetActionName(string methodName)
        {
            return methodName.HyphenCase();
        }

        public override string GetShortOptionName(string alias)
        {
            return $"-{alias}";
        }

        public override bool IsMatchingParameterName(string parameterName, string token)
        {
            return GetLongOptionName(parameterName) == token;
        }

        public override string GetNegatedLongOptionName(string parameterName)
        {
            return $"--no-{parameterName}".HyphenCase();
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