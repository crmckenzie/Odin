using System.Reflection;

namespace Odin
{
    public class DefaultConventions : Conventions
    {
        public override string GetCommandName(Command command)
        {
            return command.GetType().Name.Replace("Command", "");
        }

        public override string GetArgumentName(ParameterInfo row)
        {
            return $"--{row.Name}";
        }

        public override bool IsArgumentIdentifier(string value)
        {
            return value.StartsWith("--");
        }
    }
}