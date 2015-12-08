namespace Odin.Tests
{
    [DefaultAction("DoSomething")]
    public class NotTheDefaultController: Controller
    {
        [Action]
        public virtual void DoSomething()
        {
        }
    }
}