namespace Odin
{
    public class ValidationResult
    {
        public string CommandName { get;}
        public string[] Messages { get; }

        public ValidationResult(string commandName, string[]messages)
        {
            this.CommandName = commandName;
            this.Messages = messages;
        }
    }
}