using System.Linq;
using Odin.Parsing;

namespace Odin.Configuration
{
    public class DefaultHyphenCaseParser : IParser
    {
        private readonly ParameterValue _parameter;

        public DefaultHyphenCaseParser(ParameterValue parameter)
        {
            this._parameter = parameter;
        }

        public ParseResult Parse(string[] tokens, int i)
        {
            var token = tokens[i];

            if (ShouldParseArray(_parameter, token))
            {
                return ParseArray(_parameter, tokens);
            }

            if (ShouldParseNameValuePair(_parameter, tokens, i))
            {
                return ParseNameValuePair(_parameter, tokens, i);
            }

            if (_parameter.IsBoolean())
            {
                return new ParseResult()
                {
                    Value = !_parameter.IsNegatedBy(token),
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
                Value = _parameter.ParameterType.Coerce(token),
                TokensProcessed = 1,
            };
        }

        private bool ArgIsIdentifier(string[] tokens, int indexOfCurrentArg)
        {
            return indexOfCurrentArg < tokens.Length && HyphenCaseConvention.IsOptionIdentifier(tokens[indexOfCurrentArg]);
        }

        private static ParseResult ParseNameValuePair(ParameterValue parameter, string[] tokens, int i)
        {
            var value = tokens[i + 1];
            return new ParseResult()
            {
                Value = parameter.ParameterType.Coerce(value),
                TokensProcessed = 2,
            };
        }

        private static bool ShouldParseNameValuePair(ParameterValue parameter, string[] tokens, int i)
        {
            var token = tokens[i];
            var hasNextArgument = tokens.Length > (i + 1);
            return parameter.IsIdentifiedBy(token) && hasNextArgument;
        }

        private ParseResult ParseArray(ParameterValue parameter, string[] tokens)
        {
            var tokensToParse = tokens.Skip(1).TakeUntil(HyphenCaseConvention.IsOptionIdentifier);

            var elementType = parameter.ParameterType.GetElementType();

            var range = tokensToParse.Select(elementType.Coerce).ToArray();
            return new ParseResult()
            {
                Value = range,
                TokensProcessed = range.Length,
            };
        }

        private static bool ShouldParseArray(ParameterValue parameter, string token)
        {
            return parameter.IsIdentifiedBy(token) && parameter.ParameterType.IsArray();
        }

    }
}