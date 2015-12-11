using System.Reflection;

namespace Odin
{
    public abstract class Conventions
    {
        public abstract string GetCommandName(Command command);
        public abstract string GetArgumentName(ParameterInfo row);
        public abstract bool IsArgumentIdentifier(string value);
    }
}