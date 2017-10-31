using System;
using System.ComponentModel;
using Odin.Attributes;

namespace Odin.Demo
{
    [Description(@"This is a demo of the Odin-Commands NuGet package.
You can use this package to easily create command line applications that automatically route command-line arguments to the correct command based on customizable conventions.")]
    public class RootCommand : Command
    {

        public RootCommand() : this(new KatasCommand())
        {
        }
        

        public RootCommand(KatasCommand fizzbuzz) 
        {
            base.RegisterSubCommand(fizzbuzz);
        }

        [Action]
        [Description("The proverbial 'hello world' application.")]
        public int Hello([Description("Override who to say hello to. Defaults to 'World'.")]string who = "World")
        {
            this.Logger.Info($"Hello {who}!\n");
            return 0;
        }

        [Action]
        [Description("Display the current time")]
        public void Time([Description("When true displays the current date.")] bool showDate)
        {
            if (showDate)
            {
                this.Logger.Info($"The date is {DateTime.Today.ToShortDateString()}\n");
            }
            this.Logger.Info($"The time is {DateTime.Now.ToLocalTime():hh:mm:ss tt}\n");
        }
    }
}