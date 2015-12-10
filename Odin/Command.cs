using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Odin
{
    public abstract class Command
    {
        private readonly Dictionary<string, ActionMap> _actionMaps;
        private Dictionary<string, Command> SubCommands { get; }
        protected Command(Logger logger = null)
        {
            this._actionMaps = GetActionMaps();

            Name = GetType().Name.Replace("Controller", "");
            SubCommands = new Dictionary<string, Command>();

            if (logger != null)
            {
                _Logger = logger;
            }
            else
            {
                _Logger = new Logger();

                Logger.OnInfo += Console.Write;
                Logger.OnWarning += Console.Write;
                Logger.OnError += Console.Error.Write;
            }

            this.Description = GetDescription();
        }

        public string Description { get; }

        internal void SetParent(Command parent)
        {
            this._parent = parent;
        }

        private Command _parent;
        protected Command Parent => _parent;

        private readonly Logger _Logger;
        public Logger Logger
        {
            get
            {
                if (Parent != null)
                    return Parent.Logger;
                return _Logger;
            }
        }

        public string Name { get; set; }

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
            var attribute = this.GetType().GetCustomAttribute<System.ComponentModel.DescriptionAttribute>(inherit:true);
            if (attribute != null)
            {
                return attribute.Description;
            }
            return this.GetType().Name;
        }

        protected virtual void RegisterSubCommand(Command command)
        {
            this.SubCommands[command.Name] = command;
            command.SetParent(this);
        }

        public virtual int Execute(params string[] args)
        {
            int result = -1;

            var invocation = this.GenerateInvocation(args);
            if (invocation != null)
            {
                if (invocation.CanInvoke())
                {
                    result =  invocation.Invoke();
                }
            }

            if (result == 0)
                return result;

            this.Logger.Error("Unrecognized command sequence: {0}", string.Join(" ", args));
            this.Help();
            return result;
        }

        public ActionInvocation GenerateInvocation(string[] args)
        {
            var actionName = GetActionName(args);
            var subCommand = GetSubCommandByName(args.FirstOrDefault());

            if (IsValidActionName(actionName))
            {
                var actionMap = _actionMaps[actionName];
                var theRest = args.Skip(1).ToArray();
                var invocation = actionMap.GenerateInvocation(theRest);
                return invocation;
            }
            else if (subCommand == null)
            {
                var actionMap = _actionMaps.Values.FirstOrDefault(row => row.IsDefaultAction);
                return actionMap?.GenerateInvocation(args);
            }
            else
            {
                var theRest = args.Skip(1).ToArray();
                return subCommand.GenerateInvocation(theRest);
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
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                var actionMap = _actionMaps[actionName];
                return actionMap.Help();
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
            builder.AppendFormat("\t{0} Help <action>", this.Name)
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
            builder.AppendFormat("\t{0} <subcommand> Help", this.Name);
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