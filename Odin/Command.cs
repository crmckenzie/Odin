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
        private Dictionary<string, ActionMap> _actionMaps;
        private Dictionary<string, Command> SubCommands { get; }
        protected Command(Conventions conventions = null)
        {
            SubCommands = new Dictionary<string, Command>();
            _conventions = conventions ?? new DefaultConventions();
            this.Description = GetDescription();
        }

        public Command Use(Logger logger)
        {
            _logger = logger ?? new DefaultLogger();
            return this;
        }

        protected void InitializeActionMaps()
        {
            if (this._actionMaps != null)
                return;
            this._actionMaps = GetActionMaps();
        }

        public string Description { get; }

        internal void SetParent(Command parent)
        {
            this._parent = parent;
        }

        private Command _parent;
        protected Command Parent => _parent;

        private Logger _logger = new DefaultLogger();
        public Logger Logger => IsRoot() ? _logger : Parent.Logger;

        private readonly Conventions _conventions;
        public Conventions Conventions
        {
            get
            {
                if (Parent != null)
                    return Parent.Conventions;
                return _conventions;
            }
        }

        public virtual string Name => Conventions.GetCommandName(this);

        private Dictionary<string, ActionMap> GetActionMaps()
        {
            return GetActionMethods()
                .Select(row => new ActionMap(this, row))
                .ToDictionary(row => row.Name)
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

        protected virtual void RegisterSubCommand(Command command)
        {
            command.SetParent(this);
            this.SubCommands[command.Name] = command;
        }

        public virtual int Execute(params string[] args)
        {
            int result = -1;
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

        public ActionInvocation GenerateInvocation(string[] args)
        {
            try
            {
                this.InitializeActionMaps();

                var subCommand = GetSubCommandByName(args.FirstOrDefault());
                if (subCommand != null)
                {
                    var theRest = args.Skip(1).ToArray();
                    return subCommand.GenerateInvocation(theRest);
                }

                var actionName = GetActionName(args);
                if (IsValidActionName(actionName))
                {
                    var actionMap = _actionMaps[actionName];
                    var theRest = args.Skip(1).ToArray();
                    var invocation = actionMap.GenerateInvocation(theRest);
                    return invocation;
                }
                else
                {
                    var actionMap = _actionMaps.Values.FirstOrDefault(row => row.IsDefaultAction);
                    return actionMap?.GenerateInvocation(args);
                }

            }
            catch (ParameterConversionException pce)
            {
                this.Logger.Error(pce.Message);
                return null;
            }
        }

        private bool IsValidActionName(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                return false;

            return _actionMaps.ContainsKey(actionName);
        }

        private string GetActionName(string[] args)
        {
            var name = args.FirstOrDefault() ?? 
                _actionMaps.Values.FirstOrDefault(a => a.IsDefaultAction)?.Name;
            return name;
        }

        private Command GetSubCommandByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            return SubCommands.ContainsKey(name) ? SubCommands[name] : null;
        }

        public virtual string GenerateHelp(string actionName = "")
        {
            this.InitializeActionMaps();

            if (IsValidActionName(actionName))
            {
                var actionMap = _actionMaps[actionName];
                return actionMap.Help();
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

            if (_actionMaps.Any())
                GetMethodsHelp(builder);

            var result = builder.ToString();

            return result;
        }

        private void GetMethodsHelp(StringBuilder builder)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendLine("ACTIONS");

            foreach (var method in _actionMaps.Values.OrderBy(m => m.Name))
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