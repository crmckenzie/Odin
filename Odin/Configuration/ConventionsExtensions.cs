using System.Reflection;

namespace Odin.Configuration
{
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
        /// <param name="parameterMap"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsMatchingParameter(this IConventions conventions, ParameterValue parameterMap, string token)
        {
            return conventions.IsMatchingParameterName(parameterMap.ParameterInfo.Name, token);
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