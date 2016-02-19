using System;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Parsing;

namespace Odin.Configuration
{
    /// <summary>
    /// Provides a convention for hyphen-case command-line argument styles.
    /// </summary>
    /// <remarks>
    /// All command identifiers are lower-cased.
    /// Word breaks are denoted by -'s.
    /// Long option names begin with --
    /// Short option names begin with -
    /// </remarks>
    public class HyphenCaseConvention : IConventions
    {
        /// <summary>
        /// Returns the hyphen-cased name of the command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            var hyphenCased = name.Replace("Command", "").HyphenCase();
            return hyphenCased;
        }

        /// <summary>
        /// Returns the hyphen-cased long option name for a parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string GetLongOptionName(string parameterName)
        {
            return $"--{parameterName.HyphenCase()}";
        }

        /// <summary>
        /// Returns the hyphen-cased name of an action.
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public string GetActionName(string actionName)
        {
            return actionName.HyphenCase();
        }

        /// <summary>
        /// Returns the short version of a parameter name.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string GetShortOptionName(string alias)
        {
            return $"-{alias}";
        }

        /// <summary>
        /// Returns true of the token matches the parameter name. Otherwise false.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsMatchingParameterName(string parameterName, string token)
        {
            return GetLongOptionName(parameterName) == token;
        }

        /// <summary>
        /// Returns the negative version of a boolean switch.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string GetNegatedLongOptionName(string parameterName)
        {
            return $"--no-{parameterName.HyphenCase()}";
        }

        /// <summary>
        /// Returns a parser able to process the parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IParser CreateParser(Parameter parameter)
        {
            return new HyphenCaseParser(parameter);
        }

        /// <summary>
        /// Returns true of token represents a parameter name. Otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsParameterName(string token)
        {
            return token.StartsWith("--") || token.StartsWith("-");
        }

    }
}