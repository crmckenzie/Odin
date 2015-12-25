using System.Reflection;

namespace Odin.Configuration
{
    public static class ConventionsExtensions
    {
        public static string GetLongOptionName(this Conventions conventions, ParameterInfo parameterInfo)
        {
            return conventions.GetLongOptionName(parameterInfo.Name);
        }

        public static string GetActionName(this Conventions conventions,  MethodInfo method)
        {
            return conventions.GetActionName(method.Name);
        }

        public static bool IsMatchingParameter(this Conventions conventions, ParameterValue parameterMap, string token)
        {
            return conventions.IsMatchingParameterName(parameterMap.ParameterInfo.Name, token);
        }

        public static bool IsNegatedLongOptionName(this Conventions conventions, string parameterName, string token)
        {
            return conventions.GetNegatedLongOptionName(parameterName) == token;
        }

    }
}