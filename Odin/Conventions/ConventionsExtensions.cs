using System.Reflection;

namespace Odin.Conventions
{
    /// <summary>
    /// Provides extension methods related to the <see cref="IConventions"/> interface.
    /// </summary>
    public static class ConventionsExtensions
    {
        /// <summary>
        /// Returns the formatted action name for a given method.
        /// </summary>
        /// <param name="conventions"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetActionName(this IConventions conventions,  MethodInfo method)
        {
            return conventions.GetActionName(method.Name);
        }

        /// <summary>
        /// Returns true if the parameter is matched by the token. Otherwise false.
        /// </summary>
        /// <param name="conventions"></param>
        /// <param name="parameter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsMatchingParameter(this IConventions conventions, Parameter parameter, string token)
        {
            return conventions.IsMatchingParameterName(parameter.Name, token);
        }

        /// <summary>
        /// Returns true of the token is the negative version of a boolean switch. Otherwise false.
        /// </summary>
        /// <param name="conventions"></param>
        /// <param name="parameterName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsNegatedLongOptionName(this IConventions conventions, string parameterName, string token)
        {
            return conventions.GetNegatedLongOptionName(parameterName) == token;
        }

    }
}