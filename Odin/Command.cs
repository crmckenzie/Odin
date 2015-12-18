using System;
using System.Collections.Generic;
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
        private Dictionary<string, Command> SubCommands { get; }
        protected Command(Conventions conventions = null)
        {
            SubCommands = new Dictionary<string, Command>();
            _conventions = conventions ?? new HyphenCaseConvention();
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

        private string Description { get; }

        private void SetParent(Command parent)
        {
            this.Parent = parent;
        }

        private Command Parent { get; set; }

        private Logger _logger = new DefaultLogger();
        public Logger Logger => IsRoot() ? _logger : Parent.Logger;

        private  Conventions _conventions;
        public Conventions Conventions => IsRoot() ? _conventions : Parent.Conventions ;

        private string Name => Conventions.GetCommandName(this);

        private List<MethodInvocation> GetActions()
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
            this.SubCommands[command.Name] = command;
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

        public string GenerateHelp(string actionName = "")
        {
            var actions = this.GetActions();
            var names = actions.Select(row => row.Name);
            if (names.Contains(actionName))
            {
                var action = actions.First(row => row.Name == actionName);
                return action.Help();
            }

            if (this.SubCommands.ContainsKey(actionName))
            {
                var subCommand = this.SubCommands[actionName];
                return subCommand.GenerateHelp();
            }

            var builder = new StringBuilder();
            builder.AppendLine(this.Description);

            if (SubCommands.Any())
                GetSubCommandsHelp(builder);

            if (actions.Any())
                GetMethodsHelp(builder, actions);

            var result = builder.ToString();

            return result;
        }

        private void GetMethodsHelp(StringBuilder builder, IEnumerable<MethodInvocation>  actions)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendLine("ACTIONS");

            foreach (var method in actions.OrderBy(m => m.Name))
            {
                var methodHelp = method.Help();;
                builder.AppendLine(methodHelp);
            }

            builder.AppendLine();
            builder.AppendLine("To get help for actions");
            var helpActionName = this.Conventions.GetActionName(this.GetType().GetMethod("Help"));
            builder.AppendFormat("\t{0} {1} <action>", this.Name, helpActionName)
                .AppendLine();
        }

        private void GetSubCommandsHelp(StringBuilder builder)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendLine("SUB COMMANDS");

            foreach (var subCommand in SubCommands.Values)
            {
                builder
                    .AppendFormat("{0,-30}", subCommand.Name)
                    .AppendLine(subCommand.Description)
                    ;
            }

            builder.AppendLine();
            builder.AppendLine("To get help for subcommands");
            var helpActionName = this.Conventions.GetActionName(this.GetType().GetMethod("Help"));

            if (IsRoot())
            {
                builder.AppendFormat("\t{0} <subcommand>", helpActionName);
            }
            else
            {
                var fullPath = GetFullCommandPath().Skip(1); // remove the root command from the path.
                var path = string.Join(" ", fullPath);
                builder.AppendFormat("\t{0} {1} <subcommand>", path, helpActionName);
            }

        }

        private string[] GetFullCommandPath()
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

        private bool IsRoot()
        {
            return this.Parent == null;
        }

        [Action]
        public void Help(
            [Description("The name of the action to provide help for.")]
            string actionName = "")
        {
            var help = this.GenerateHelp(actionName);
            this.Logger.Info(help);
        }
    }
}