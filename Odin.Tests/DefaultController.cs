using System.ComponentModel;

namespace Odin.Tests
{
    [DefaultAction("DoSomething")]
    [Description("This is the default controller")]
    public class DefaultController : Controller
    {
        public object[] MethodArguments { get; set; }

        public DefaultController() : this(new SubCommandController(), new Logger())
        {
           
        }

        public DefaultController(SubCommandController subcommand, Logger logger)
        {
            var subcommand1 = subcommand ?? new SubCommandController();

            Logger = logger ?? new Logger();

            base.RegisterSubCommand(subcommand1);

            base.Logger = logger ?? base.Logger;
        }

        public void NotAnAction()
        {
            
        }

        [Action]
        public virtual void DoSomething()
        {

        }

        [Action]
        public virtual int AlwaysReturnsMinus2()
        {
            return -2;
        }

        [Action]
        public virtual void SomeOtherControllerAction()
        {
            
        }

        [Action]
        public virtual void WithRequiredStringArg(string argument)
        {
            MethodArguments = new object[] {argument};
        }

        [Action]
        public void WithRequiredStringArgs(string argument1, string argument2)
        {
            MethodArguments = new object[] { argument1, argument2 };
        }

        [Action]
        public void WithOptionalStringArg(string argument = "not-passed")
        {
            MethodArguments = new object[] { argument };
        }

        [Action]
        public void WithOptionalStringArgs(string argument1 = "value1-not-passed", string argument2 = "value2-not-passed", string argument3 = "value3-not-passed")
        {
            MethodArguments = new object[] { argument1, argument2, argument3 };
        }

        [Action]
        public void WithSwitch(bool argument)
        {
            MethodArguments = new object[] { argument };
        }
    }
}