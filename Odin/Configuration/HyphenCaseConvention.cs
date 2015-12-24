using System.Linq;
using System.Reflection;
using Odin.Attributes;

namespace Odin.Configuration
{
    public class HyphenCaseConvention : Conventions
    {
        public override string GetCommandName(Command command)
        {
            var name = command.GetType().Name;
            var hyphenCased = name.Replace("Command", "").HyphenCase();
            return hyphenCased;
        }

        public override string GetLongOptionName(ParameterInfo row)
        {
            return $"--{row.Name.HyphenCase()}";
        }

        public override string GetActionName(MethodInfo methodInfo)
        {
            return methodInfo.Name.HyphenCase();
        }

        public override string GetShortOptionName(string rawAlias)
        {
            return $"-{rawAlias}";
        }

        public override bool IsMatchingParameter(ParameterValue parameterMap, string arg)
        {
            return parameterMap.LongOptionName == arg;
        }

        private bool IsOptionIdentifier(string value)
        {
            return value.StartsWith("--");
        }

        private bool ArgIsIdentifier(string[] tokens, int indexOfCurrentArg)
        {
            return indexOfCurrentArg < tokens.Length && IsOptionIdentifier(tokens[indexOfCurrentArg]);
        }

        public override ParseResult Parse(ParameterValue parameter, string[] tokens, int i)
        {
            var token = tokens[i];

            if (parameter.IsIdentifiedBy(token) && parameter.IsArray())
            {
                var tokensToParse = tokens.Skip(1).TakeUntil(t => IsOptionIdentifier(t) || IsOptionIdentifier(t));

                var range = tokensToParse.Select(parameter.Coerce).ToArray();
                return new ParseResult()
                {
                    Value = range,
                    TokensProcessed = range.Length,
                };
            }


            var hasNextArg = tokens.Length > (i + 1);
            if (parameter.IsIdentifiedBy(token) && hasNextArg)
            {
                var value = tokens[i + 1];
                return new ParseResult()
                {
                    Value = parameter.Coerce(value),
                    TokensProcessed = 2,
                };
            }

            if (parameter.IsBoolean())
            {
                return new ParseResult()
                {
                    Value = true,
                    TokensProcessed = 1,
                };
            }

            if (ArgIsIdentifier(tokens, i + 1))
            {
                return new ParseResult()
                {
                    TokensProcessed = 0
                };
            }

            return new ParseResult()
            {
                Value = parameter.Coerce(token),
                TokensProcessed = 1,
            };
        }

    }
}