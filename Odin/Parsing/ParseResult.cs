namespace Odin.Parsing
{
    /// <summary>
    /// The result of a parameter value parsing attempt.
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Gets or sets the parsed value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the number of tokens processed.
        /// </summary>
        public int TokensProcessed { get; set; }
    }
}