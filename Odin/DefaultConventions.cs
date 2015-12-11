using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Odin
{
    public static class StringExtensions
    {
        public static string HyphenCase(this string input)
        {
            var result = Regex.Replace(input, ".[A-Z]", m => m.Value[0] + "-" + m.Value[1]).ToLower();
            return result;
        }
    }

    public class DefaultConventions : Conventions
    {

        private string GetTableName(Type type)
        {
            var name = Regex.Replace(type.Name, "Entity$", "");
            return name.HyphenCase().ToUpper();
        }

        public override string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            var hyphenCased = name.Replace("Command", "").HyphenCase();
            return hyphenCased;
        }

        public override string GetArgumentName(ParameterInfo row)
        {
            return $"--{row.Name.HyphenCase()}";
        }

        public override bool IsArgumentIdentifier(string value)
        {
            return value.StartsWith("--");
        }

        public override string GetActionName(MethodInfo methodInfo)
        {
            return methodInfo.Name.HyphenCase();
        }
    }
}