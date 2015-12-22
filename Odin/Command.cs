using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Exceptions;
using Odin.Logging;

namespace Odin
{
    public abstract class Command
    {
        private Dictionary<string, Command> _subCommands; 
        public IReadOnlyDictionary<string, Command> SubCommands { get; }



        protected Command()
        {
            _subCommands = new Dictionary<string, Command>();
            SubCommands =new ReadOnlyDictionary<string, Command>(_subCommands);

            _conventions = new HyphenCaseConvention();
            _helpGenerator = new HelpGenerator();
            this.Description = GetDescription();
        }

        public Command Use(Conventions conventions)
        {
            _conventions = conventions;
            return this;
        }

        public Command Use(Logger logger)
        {
            _logger = logger ?? new DefaultLogger();
            return this;
        }

        public string Description { get; }

        private void SetParent(Command parent)
        {
            this.Parent = parent;
        }

        private Command Parent { get; set; }

        private Logger _logger = new DefaultLogger();
        public Logger Logger => IsRoot() ? _logger : Parent.Logger;

        private  Conventions _conventions;
        private readonly HelpGenerator _helpGenerator;

        public HelpGenerator HelpGenerator => IsRoot() ? _helpGenerator : Parent.HelpGenerator;

        public Conventions Conventions => IsRoot() ? _conventions : Parent.Conventions ;

        public string Name => Conventions.GetCommandName(this);

        public List<MethodInvocation> GetActions()
        {
            return GetActionMethods()
                .Select(row => new MethodInvocation(this, row))
                .ToList()
                ;
        }

        private IEnumerable<MethodInfo> GetActionMethods()
        {
            return this
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ActionAttribute>() != null)
                ;
        }

        private string GetDescription()
        {
            var defaultDescription = IsRoot() ? "" : this.Name;
            var attribute = this.GetType().GetCustomAttribute<DescriptionAttribute>(inherit:true);
            return attribute != null ? attribute.Description : defaultDescription;
        }

        public Command RegisterSubCommand(Command command)
        {
            command.SetParent(this);
            this._subCommands[command.Name] = command;
            return this;
        }

        public int Execute(params string[] args)
        {
            var result = -1;
            var invocation = this.GenerateInvocation(args);
            if (invocation?.CanInvoke() == true)
            {
                result =  invocation.Invoke();
            }

            if (result == 0)
                return result;

            this.Logger.Error("Unrecognized command sequence: {0}\n", string.Join(" ", args));
            this.Help();
            return result;
        }

        public MethodInvocation GenerateInvocation(params string[] tokens)
        {
            try
            {
                var commandOrActionName = tokens.FirstOrDefault();
                var subCommand = GetSubCommandByName(commandOrActionName);
                if (subCommand != null)
                {
                    var theRest = tokens.Skip(1).ToArray();
                    return subCommand.GenerateInvocation(theRest);
                }

                var actions = this.GetActions();
                var action = actions.FirstOrDefault(row => row.Name == commandOrActionName);
                var toSkip = 1;
                if (action == null)
                {
                    toSkip = 0;
                    action = actions.FirstOrDefault(row => row.IsDefault);
                }
 
                var args = tokens.Skip(toSkip).ToArray();
                action?.SetParameterValues(args);
                return action;
            }
            catch (ParameterConversionException pce)
            {
                this.Logger.Error(pce.Message);
                return null;
            }
        }

        private Command GetSubCommandByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return SubCommands.ContainsKey(name) ? SubCommands[name] : null;
        }

        public string[] GetFullCommandPath()
        {
            var stack = new Stack<string>();
            stack.Push(this.Name);

            var parent = this.Parent;
            while (parent != null)
            {
                stack.Push(parent.Name);
                parent = parent.Parent;
            }

            return stack.ToArray();
        }

        public bool IsRoot()
        {
            return this.Parent == null;
        }

        [Action]
        public void Help(
            [Description("The name of the action to provide help for.")]
            string actionName = "")
        {
            var help = this.HelpGenerator.GenerateHelp(this, actionName);
            this.Logger.Info(help);
        }
    }
}