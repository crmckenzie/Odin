using System.ComponentModel;
using Odin.Attributes;

namespace Odin.Tests.Lib
{

    [Description("This is the default command")]
    public class DefaultCommand : Command
    {
        public object[] MethodArguments { get; set; }

        public DefaultCommand() 
        {
            
        }

        public DefaultCommand(SubCommand subcommand) 
        {
            var subcommand1 = subcommand ?? new SubCommand();
            base.RegisterSubCommand(subcommand1);
        }

        public void NotAnAction()
        {
            // this method is here to demonstrate that Odin only inteprets
            // commands for actions decorated with the `[Action]` attribute.
        }

        [Action(IsDefault = true)]
        [Description("A description of the DoSomething() method.")]
        public void DoSomething(
            [Alias("a", "A")]
            [Description("Lorem ipsum dolor sit amet, consectetur adipiscing elit")]
            string argument1 = "value1-not-passed",
            [Alias("b", "B")]
            [Description("sed do eiusmod tempor incididunt ut labore et dolore magna aliqua")]
            string argument2 = "value2-not-passed",
            [Alias("c", "C")]
            [Description("Ut enim ad minim veniam")]
            string argument3 = "value3-not-passed")
        {
            this.MethodArguments = new object[] {argument1, argument2, argument3};
        }

        [Action]
        public virtual int AlwaysReturnsMinus2()
        {
            return -2;
        }

        [Action]
        public virtual bool AlwaysReturnsTrue()
        {
            return true;
        }

        [Action]
        public virtual bool AlwaysReturnsFalse()
        {
            return false;
        }


        [Action]
        public virtual void SomeOtherControllerAction()
        {
            
        }

        [Action]
        public virtual void WithRequiredStringArg(string argument)
        {
            this.MethodArguments = new object[] {argument};
        }

        [Action]
        public void WithRequiredStringArgs(string argument1, string argument2)
        {
            this.MethodArguments = new object[] { argument1, argument2 };
        }

        [Action]
        public void WithOptionalStringArg(string argument = "not-passed")
        {
            this.MethodArguments = new object[] { argument };
        }

        [Action]
        public void WithOptionalStringArgs(
            string argument1 = "value1-not-passed", 
            string argument2 = "value2-not-passed", 
            string argument3 = "value3-not-passed")
        {
            this.MethodArguments = new object[] { argument1, argument2, argument3 };
        }

        [Action]
        public void WithSwitch(bool argument)
        {
            this.MethodArguments = new object[] { argument };
        }

        [Action]
        public void WithArgumentsOfVariousTypes(int i, long j)
        {
            
        }
    }
}