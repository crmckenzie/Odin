using System.Reflection;
using Odin.Parsing;

namespace Odin.Configuration
{
    public class SlashEqualsConvention : IConventions
    {
        public string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            return name.Replace("Command", "");
        }

        public string GetLongOptionName(string parameterName)
        {
            return $"/{parameterName}";
        }

        public string GetNegatedLongOptionName(string parameterName)
        {
            return $"/no-{parameterName}";
        }

        public string GetActionName(string actionName)
        {
            return actionName;
        }

        public string GetShortOptionName(string alias)
        {
            return $"/{alias}";
        }

        public bool IsMatchingParameterName(string parameterName, string token)
        {
            return token.StartsWith(GetLongOptionName(parameterName));
        }

        public IParser CreateParser(ParameterValue parameter)
        {
            return new SlashEqualsParser(parameter);
        }

        public bool IsParameterName(string token)
        {
            return token.StartsWith("/");
        }
    }
}