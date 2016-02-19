using System.Reflection;
using System.Threading;
using Odin.Attributes;
using Odin.Parsing;

namespace Odin.Configuration
{
    /// <summary>
    /// Convention implementation for args of the form /name:value
    /// </summary>
    public class SlashColonConvention : IConventions
    {
        /// <summary>
        /// Gets the conventional name of the command.
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
        /// Gets the conventional negative form of the long option name for a boolean switch.
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
        /// True if the parameter name matches the token by convention. Otherwise false.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsMatchingParameterName(string parameterName, string token)
        {
            return token.StartsWith(GetLongOptionName(parameterName));
        }

        /// <summary>
        /// Returns the correct convention-based parser for the parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IParser CreateParser(Parameter parameter)
        {
            return new SlashColonParser(parameter);
        }

        /// <summary>
        /// True if the token represents a parameter name. Otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsParameterName(string token)
        {
            return token.StartsWith("/");
        }
    }
}