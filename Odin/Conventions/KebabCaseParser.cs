using System;
using System.Linq;
using Odin.Parsing;

namespace Odin.Conventions
{
    /// <summary>
    /// Provides parsing capabilities for hyphen-cased parameters.
    /// </summary>
    public class KebabCaseParser : IParser
    {
        private readonly Parameter _parameter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameter"></param>
        public KebabCaseParser(Parameter parameter)
        {
            this._parameter = parameter;
        }

        /// <summary>
        /// Returns a <see cref="ParseResult"/> given a position in a list of tokens.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="tokenIndex"></param>
        /// <returns></returns>
        public ParseResult Parse(string[] tokens, int tokenIndex)
        {
            var token = tokens[tokenIndex];

            if (ShouldParseArray(_parameter, token))
            {
                return ParseArray(_parameter, tokens);
            }

            if (ShouldParseNameValuePair(_parameter, tokens, tokenIndex))
            {
                return ParseNameValuePair(_parameter, tokens, tokenIndex);
            }

            if (_parameter.IsBoolean())
            {
                return new ParseResult()
                {
                    Value = !_parameter.IsNegatedBy(token),
                    TokensProcessed = 1,
                };
            }

            if (ArgIsIdentifier(tokens, tokenIndex))
            {
                return new ParseResult()
                {
                    TokensProcessed = 0
                };
            }

            return new ParseResult()
            {
                Value = _parameter.Coerce(token),
                TokensProcessed = 1,
            };
        }

        private bool ArgIsIdentifier(string[] tokens, int indexOfCurrentArg)
        {
            var tokenIsInRange = indexOfCurrentArg < tokens.Length;
            if (!tokenIsInRange) return false;

            var token = tokens[indexOfCurrentArg];
            return _parameter.IsParameterName(token);
        }

        private static ParseResult ParseNameValuePair(Parameter parameter, string[] tokens, int i)
        {
            var value = tokens[i + 1];
            return new ParseResult()
            {
                Value = parameter.Coerce(value),
                TokensProcessed = 2,
            };
        }

        private static bool ShouldParseNameValuePair(Parameter parameter, string[] tokens, int i)
        {
            var token = tokens[i];
            var hasNextArgument = tokens.Length > (i + 1);
            return parameter.IsIdentifiedBy(token) && hasNextArgument;
        }

        private ParseResult ParseArray(Parameter parameter, string[] tokens)
        {
            var tokensToParse = tokens
                .Skip(1)
                .TakeUntil(_parameter.IsParameterName);

            var elementType = parameter.ParameterType.GetElementType();

            var range = tokensToParse.Select(elementType.Coerce).ToArray();
            var value = Array.CreateInstance(elementType, range.Length);
            Array.Copy(range, value, range.Length);

            return new ParseResult()
            {
                Value = value,
                TokensProcessed = range.Length+1,
            };
        }

        private static bool ShouldParseArray(Parameter parameter, string token)
        {
            return parameter.IsIdentifiedBy(token) && parameter.ParameterType.IsArray();
        }

    }
}