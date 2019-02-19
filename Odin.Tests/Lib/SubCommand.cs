using System.Diagnostics;

namespace Odin.Tests.Lib
{
    using System.ComponentModel;

    using Odin.Attributes;

    [Description("Provides a component of testability for subcommands.")]
    public class SubCommand : Command
    {

        [Action(IsDefault = true)]
        public virtual void DoSomething()
        {
            this.LogMethodCall();
            this.Logger.Info("Do some SubCommand stuff!");
        }

        private void LogMethodCall(params object[] arguments)
        {
            var stackTrace = new StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            this.MethodCalled = methodBase.Name;
            this.MethodArguments = arguments;
        }

        public object[] MethodArguments { get; set; }

        public string MethodCalled { get; set; }
    }
}
