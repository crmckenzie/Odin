namespace Odin
{
    /// <summary>
    /// Communications validation information about the command structure.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets the name of the command with the validation issue.
        /// </summary>
        public string CommandName { get;}

        /// <summary>
        /// Gets a list of validation issues for the command.
        /// </summary>
        public string[] Messages { get; }

        public ValidationResult(string commandName, string[]messages)
        {
            this.CommandName = commandName;
            this.Messages = messages;
        }
    }
}