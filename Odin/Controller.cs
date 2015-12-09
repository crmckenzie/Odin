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

        public Logger Logger { get; set; }

        protected Controller()
        {
            // order matters with these assignments
            this._defaultActionAttribute = GetType().GetCustomAttribute(typeof(DefaultActionAttribute)) as DefaultActionAttribute;
            this._actionMaps = GetActionMaps();

            Name = GetType().Name.Replace("Controller", "");
            SubCommands = new Dictionary<string, Controller>();
            Logger = new Logger();
            this.Description = GetDescription();
        }

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

        public string Description { get; }

        protected virtual void RegisterSubCommand(Controller controller)
        {
            this.SubCommands[controller.Name] = controller;
        }

        public string Name { get; set; }

        public virtual int Execute(string[] args)
        {
            if (args.Any())
            {
                var first = args.First();
                if (_actionMaps.ContainsKey(first))
                {
                    var result = InvokeMethod(first, args.Skip(1).ToArray());
                    if (result < 0) this.Help();
                    return result;
                }

                var subCommand = GetSubCommandByName(first);
                if (subCommand != null)
                {
                    var theRest = args.Skip(1).ToArray();
                    return subCommand.Execute(theRest);
                }

                this.Logger.Error("Unrecognized command sequence: {0}", string.Join(" ", args));
            }
            else if (_defaultActionAttribute != null)
            {
                var result = InvokeMethod(_defaultActionAttribute.MethodName, args);
                if (result < 0) this.Help();
                return result;
            }

            this.Help();
            return -1;
        }

        public IEnumerable<ParameterMap> Map(string[] args)
        {
            ParameterMap map = null;
            foreach (var arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    if (map != null)
                    {
                        yield return map;
                    }

                    map = new ParameterMap()
                    {
                        Switch = arg
                    };
                }
                else
                {
                    map?.RawValues.Add(arg);
                }
            }

            if (map != null)
                yield return map;
        }

        private int InvokeMethod(string name, string[] args)
        {
            var actionMap = _actionMaps[name];

            var invocation = actionMap.GenerateInvocation(args);
            if (!invocation.CanInvoke()) return -1;

            var result = invocation.Invoke();
            return result;
        }

        private Controller GetSubCommandByName(string name)
        {
            if (SubCommands.ContainsKey(name))
            {
                return SubCommands[name];
            }
            return null;
        }

        public virtual string GenerateHelp(string actionName = "")
        {
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                var actionMap = _actionMaps[actionName];
                return actionMap.Help();
            }

            var builder = new System.Text.StringBuilder();
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