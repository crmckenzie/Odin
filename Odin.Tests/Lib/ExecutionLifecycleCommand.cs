namespace Odin.Tests.Lib
{
    using System;

    using Odin.Attributes;

    public class ExecutionLifecycleCommand : Command
    {
        [Parameter]
        public DateTime Before { get; set; }
        [Parameter]
        public DateTime Begin { get; set; }
        [Parameter]
        public DateTime After { get; set; }

        protected override void OnBeforeExecute(MethodInvocation invocation)
        {
            this.Before = DateTime.Now;
        }

        protected override int OnAfterExecute(MethodInvocation invocation, int exitCode)
        {
            this.After = DateTime.Now;
            return base.OnAfterExecute(invocation, exitCode);
        }

        [Action]
        public void DoStuff()
        {
            System.Threading.Thread.Sleep(100);
            this.Begin = DateTime.Now;
            System.Threading.Thread.Sleep(100);
        }
    }
}