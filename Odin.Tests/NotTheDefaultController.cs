namespace Odin.Tests
{
    [DefaultAction("DoSomething")]
    public class NotTheDefaultController: Controller
    {
        public virtual void DoSomething()
        {
        }
    }
}