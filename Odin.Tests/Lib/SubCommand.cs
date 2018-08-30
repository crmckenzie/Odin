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
            this.Logger.Info("Do some SubCommand stuff!");
        }
    }
}
