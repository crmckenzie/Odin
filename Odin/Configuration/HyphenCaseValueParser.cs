namespace Odin.Configuration
{
    internal class HyphenCaseValueParser : IValueParser
    {
        private readonly ParameterValue _parameter;

        public HyphenCaseValueParser(Conventions conventions, ParameterValue parameter) 
        {
            this.Conventions = conventions;
            this._parameter = parameter;
        }

        private Conventions Conventions { get; set; }

        private bool IsOptionIdentifier(string value)
        {
            return value.StartsWith("--");
        }

        private bool ArgIsIdentifier(string[] tokens, int indexOfCurrentArg)
        {
            return indexOfCurrentArg < tokens.Length && IsOptionIdentifier(tokens[indexOfCurrentArg]);
        }

        public ParseResult Parse(string[] tokens, int i)
        {
            var token = tokens[i];

            var hasNextArg = tokens.Length > (i + 1);
            if (_parameter.IsIdentifiedBy(token) && hasNextArg)
            {
                var value = tokens[i + 1];
                return new ParseResult()
                {
                    Value = _parameter.Coerce(value),
                    TokensProcessed = 2,
                };
            }

            if (_parameter.IsBoolean())
            {
                return new ParseResult()
                {
                    Value = true,
                    TokensProcessed = 1,
                };
            }

            if (ArgIsIdentifier(tokens, i+1))
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
    }
}