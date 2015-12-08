using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Odin
{
    public abstract class Controller
    {
        private readonly Dictionary<string, MethodInfo> methods;
        private readonly DefaultActionAttribute defaultActionAttribute;
        private Dictionary<string, Controller> SubCommands { get; }

        public Logger Logger { get; set; }

        protected Controller()
        {
            this.methods = GetActionMethods().ToDictionary(row => row.Name);
            this.defaultActionAttribute = GetType().GetCustomAttribute(typeof(DefaultActionAttribute)) as DefaultActionAttribute;
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
                if (methods.ContainsKey(first))
                {
                    var result = InvokeMethod(first, args.Skip(1).ToArray());
                    if (result < 0) this.Help();
                    return result;
                }

                var subCommand = GetControllerByName(first);
                if (subCommand != null)
                {
                    var theRest = args.Skip(1).ToArray();
                    return subCommand.Execute(theRest);
                }

                this.Logger.Error("Unrecognized command sequence: {0}", string.Join(" ", args));
            }
            else if (defaultActionAttribute != null)
            {
                var result = InvokeMethod(defaultActionAttribute.MethodName, args);
                if (result < 0) this.Help();
                return result;
            }

            this.Help();
            return -1;
        }

        public IEnumerable<ParameterMap> Map(string[] args)
        {
            ParameterMap map = null;
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
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
                } else if (map != null)
                {
                    map.RawValues.Add(arg);
                }
            }

            if (map != null)
                yield return map;
        } 

        private int InvokeMethod(string name, string[] args)
        {
            var methodInfo = methods[name];

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

        private Controller GetControllerByName(string name)
        {
            if (SubCommands.ContainsKey(name))
            {
                return SubCommands[name];
            }
            return null;
        }

        public virtual string GenerateHelp(string actionOrSubCommand = "")
        {
            var builder = new System.Text.StringBuilder();

            builder.Append(this.Description);

            var result = builder.ToString();

            return result;
        }

        public void Help(string actionOrSubCommand = "")
        {
            this.Logger.Info(this.GenerateHelp(actionOrSubCommand));
        }
    }
}