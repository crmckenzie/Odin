namespace Odin.Tests.Lib
{
    using Odin.Attributes;

    public class NotTheDefaultCommand: Command
    {
        [Action(IsDefault = true)]
        public virtual void DoSomething()
        {
        }
    }
}