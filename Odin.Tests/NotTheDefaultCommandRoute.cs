namespace Odin.Tests
{
    [DefaultAction("DoSomething")]
    public class NotTheDefaultCommandRoute: CommandRoute
    {
        [Action]
        public virtual void DoSomething()
        {
        }
    }
}