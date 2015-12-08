namespace Odin.Tests
{
    [DefaultAction("DoSomething")]
    public class DefaultController : Controller
    {
        private readonly SubCommandController _subcommand;

        public object[] MethodArguments { get; set; }

        public DefaultController() : this(null)
        {
            
        }

        public DefaultController(SubCommandController subcommand = null)
        {
            if (subcommand == null)
                subcommand = new SubCommandController();
            _subcommand = subcommand;

            RegisterSubCommand(_subcommand);
        }

        public virtual void DoSomething()
        {

        }

        public virtual void SomeOtherControllerAction()
        {
            
        }

        public virtual void WithRequiredStringArg(string argument)
        {
            MethodArguments = new object[] {argument};
        }

        public void WithRequiredStringArgs(string argument1, string argument2)
        {
            MethodArguments = new object[] { argument1, argument2 };
        }

        public void WithOptionalStringArg(string argument = "not-passed")
        {
            MethodArguments = new object[] { argument };
        }

        public void WithOptionalStringArgs(string argument1 = "value1-not-passed", string argument2 = "value2-not-passed", string argument3 = "value3-not-passed")
        {
            MethodArguments = new object[] { argument1, argument2, argument3 };
        }
    }
}