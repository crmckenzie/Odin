using Odin.Parsing;

namespace Odin.Conventions
{
    /// <summary>
    /// Provides an interface for Command authors to customize the conventions
    /// used for routing and parsing commands and parameters.
    /// </summary>
    public interface IConventions
    {
        /// <summary>
        /// Returns the conventional name of the command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string GetCommandName(Command command);

        /// <summary>
        /// Returns the conventional name of the action.
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        string GetActionName(string actionName);

        /// <summary>
        /// Returns the conventional long name for a parameter.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        string GetLongOptionName(string parameterName);

        /// <summary>
        /// Returns the negative form of a boolean switch name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        string GetNegatedLongOptionName(string parameterName);

        /// <summary>
        /// Returns the conventional short name for a parameter.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        string GetShortOptionName(string alias);

        /// <summary>
        /// True if the parameterName matches the token by convention. Otherwise false.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsMatchingParameterName(string parameterName, string token);

        /// <summary>
        /// Returns a parser capable of parsing the value for a given parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IParser CreateParser(Parameter parameter);

        /// <summary>
        /// True if token represents a conventional parameter name. Otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsParameterName(string token);
    }
}