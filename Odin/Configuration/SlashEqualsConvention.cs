using System.Reflection;
using Odin.Parsing;

namespace Odin.Configuration
{
    public class SlashEqualsConvention : Conventions
    {
        public override string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            return name.Replace("Command", "");
        }

        public override string GetLongOptionName(string parameterName)
        {
            return $"/{parameterName}";
        }

        public override string GetNegatedLongOptionName(string parameterName)
        {
            return $"/No{parameterName}";
        }

        public override string GetActionName(string methodName)
        {
            return methodName;
        }

        public override string GetShortOptionName(string alias)
        {
            return $"/{alias}";
        }

        public override bool IsMatchingParameterName(string parameterName, string token)
        {
            return token.StartsWith(GetLongOptionName(parameterName));
        }

        public override IParser CreateParser(ParameterValue parameter)
        {
            return new SlashEqualsParser(parameter);
        }


    }
}