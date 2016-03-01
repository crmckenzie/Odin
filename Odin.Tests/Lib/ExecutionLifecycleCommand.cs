using System;
using Odin.Attributes;

namespace Odin.Tests
{
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
            Before = DateTime.Now;
        }

        protected override int OnAfterExecute(MethodInvocation invocation, int result)
        {
            After = DateTime.Now;
            return base.OnAfterExecute(invocation, result);
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