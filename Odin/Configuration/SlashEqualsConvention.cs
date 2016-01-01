using System.Reflection;
using Odin.Parsing;

namespace Odin.Configuration
{
    /// <summary>
    /// Convention implementation for args of the form /name=value
    /// </summary>
    public class SlashEqualsConvention : IConventions
    {
        /// <summary>
        /// Gets the conventional name for the command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            return name.Replace("Command", "");
        }

        /// <summary>
        /// Gets the conventional long option name for a parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string GetLongOptionName(string parameterName)
        {
            return $"/{parameterName}";
        }

        /// <summary>
        /// Gets the conventional negated long option name for a boolean switch.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string GetNegatedLongOptionName(string parameterName)
        {
            return $"/no-{parameterName}";
        }

        /// <summary>
        /// Gets the conventional action name for an action.
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public string GetActionName(string actionName)
        {
            return actionName;
        }

        /// <summary>
        /// Gets the conventional short option name for a parameter alias.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string GetShortOptionName(string alias)
        {
            return $"/{alias}";
        }

        /// <summary>
        /// True if the token matches the parameterName by convention. Otherwise false.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsMatchingParameterName(string parameterName, string token)
        {
            return token.StartsWith(GetLongOptionName(parameterName));
        }

        /// <summary>
        /// Returns the correct parser for the parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IParser CreateParser(ParameterValue parameter)
        {
            return new SlashEqualsParser(parameter);
        }

        /// <summary>
        /// True if the token represents a parameter name by convention.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsParameterName(string token)
        {
            return token.StartsWith("/");
        }
    }
}