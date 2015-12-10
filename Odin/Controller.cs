using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Odin
{
    public abstract class Controller
    {
        private readonly Dictionary<string, ActionMap> _actionMaps;
        private readonly DefaultActionAttribute _defaultActionAttribute;
        private Dictionary<string, Controller> SubCommands { get; }
        protected Controller()
        {
            // order matters with these assignments
            this._defaultActionAttribute = GetType()
                .GetCustomAttribute<DefaultActionAttribute>();

            this._actionMaps = GetActionMaps();

            Name = GetType().Name.Replace("Controller", "");
            SubCommands = new Dictionary<string, Controller>();
            Logger = new Logger();
            this.Description = GetDescription();
        }

        public string Description { get; }

        public Logger Logger { get; set; }

        public string Name { get; set; }

        private Dictionary<string, ActionMap> GetActionMaps()
        {
            return GetActionMethods()
                .Select(row => new ActionMap(this, row, IsDefaultAction(row)))
                .ToDictionary(row => row.Name)
                ;
        }

        private bool IsDefaultAction(MethodInfo methodInfo)
        {
            if (_defaultActionAttribute == null) return false;
            return _defaultActionAttribute.MethodName == methodInfo.Name;
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

        protected virtual void RegisterSubCommand(Controller controller)
        {
            this.SubCommands[controller.Name] = controller;
        }

        public virtual int Execute(string[] args)
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
            var isValid = IsValidActionName(actionName);
            if (isValid)
            {
                var actionMap = _actionMaps[actionName];
                var theRest = args.Skip(1).ToArray();
                var invocation = actionMap.GenerateInvocation(theRest);
                return invocation;
            }
            else
            {
                var subCommand = GetSubCommandByName(args.FirstOrDefault());
                if (subCommand == null) return null;

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
            var name = args.FirstOrDefault() ?? _defaultActionAttribute?.MethodName;
            return name;
        }

        private Controller GetSubCommandByName(string name)
        {
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