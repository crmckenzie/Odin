using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Odin
{
    public abstract class Controller
    {
        private readonly Dictionary<string, MethodInfo> _methods;
        private readonly DefaultActionAttribute _defaultActionAttribute;
        private Dictionary<string, Controller> SubCommands { get; }

        public Logger Logger { get; set; }

        protected Controller()
        {
            this._methods = GetActionMethods().ToDictionary(row => row.Name);
            this._defaultActionAttribute = GetType().GetCustomAttribute(typeof(DefaultActionAttribute)) as DefaultActionAttribute;
            Name = GetType().Name.Replace("Controller", "");
            SubCommands = new Dictionary<string, Controller>();
            Logger = new Logger();
            this.Description = GetDescription();
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
                if (_methods.ContainsKey(first))
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


        private IEnumerable<ParameterMap> Map(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Select(row => new ParameterMap()
            {
                ParameterInfo = row,
                Switch = $"--{row.Name}",
            });
        }

        private int InvokeMethod(string name, string[] args)
        {
            var methodInfo = _methods[name];

            var parameters = methodInfo.GetParameters().OrderBy(p => p.Position).ToArray();
            var parameterMap = Map(args).ToList();

            Merge(parameters, parameterMap);

            var orderedMaps = parameterMap.OrderBy(row => row.ParameterInfo.Position).ToArray();
            var values = orderedMaps.Select(row => row.GetValue()).ToArray();

            var result = methodInfo.Invoke(this, values);

            if (result is int)
            {
                return (int) result;
            }

            return 0;
        }

        private void Merge(ParameterInfo[] parameters, List<ParameterMap> parameterMaps)
        {
            foreach (var parameter in parameters)
            {
                var parameterMap = parameterMaps.SingleOrDefault(row => row.IsMatch(parameter));
                if (parameterMap != null)
                {
                    parameterMap.ParameterInfo = parameter;
                }
                else
                {
                    parameterMaps.Add(new ParameterMap()
                    {
                        ParameterInfo = parameter
                    });
                }
            }
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
                var method = _methods[actionName];
                var help = GetHelpForMethod(method);
                return help;
            }

            var builder = new System.Text.StringBuilder();
            builder.AppendLine(this.Description);

            if (SubCommands.Any())
                GetSubCommandsHelp(builder);

            if (_methods.Any())
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

            foreach (var method in _methods.Values.OrderBy(m => m.Name))
            {
                var methodHelp = GetHelpForMethod(method);
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

        private string GetHelpForMethod(MethodInfo method)
        {
            var builder = new System.Text.StringBuilder();
            var isDefaultAction = method.Name == _defaultActionAttribute?.MethodName;
            var name = isDefaultAction ? $"{method.Name} (default)" : method.Name;

            builder.AppendLine($"{name,-30}{GetMethodDescription(method)}");

            var parameterMaps = Map(method);
            foreach (var parameterMap in parameterMaps)
            {
                var description = parameterMap.GetDescription();
                builder.AppendLine($"\t{parameterMap.Switch,-26}{description}");
            }
            return builder.ToString();
        }


        private string GetMethodDescription(MethodInfo method)
        {
            var descriptionAttr = method.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr == null)
                return "";

            return descriptionAttr.Description;
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