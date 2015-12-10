namespace Odin.Tests
{
    public class NotTheDefaultCommand: Command
    {
        [Action(IsDefault = true)]
        public virtual void DoSomething()
        {
        }
    }
}