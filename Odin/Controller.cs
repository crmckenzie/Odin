using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Odin
{
    public abstract class Controller
    {
        private Dictionary<string, MethodInfo> methods;
        private DefaultActionAttribute defaultActionAttribute;
        private Dictionary<string, Controller> SubCommands { get; }

        protected Controller()
        {
            this.methods = GetType().GetMethods().ToDictionary(row => row.Name);
            this.defaultActionAttribute = GetType().GetCustomAttribute(typeof(DefaultActionAttribute)) as DefaultActionAttribute;
            Name = GetType().Name.Replace("Controller", "");
            SubCommands = new Dictionary<string, Controller>();
        }

        protected virtual void RegisterSubCommand(Controller controller)
        {
            this.SubCommands[controller.Name] = controller;
        }


        public string Name { get; set; }

        public virtual void Execute(string[] args)
        {
            if (args.Any())
            {
                var first = args.First();
                if (methods.ContainsKey(first))
                {
                    InvokeMethod(first, args.Skip(1).ToArray());
                    return;
                }

                var subCommand = GetControllerByName(first);
                if (subCommand != null)
                {
                    var theRest = args.Skip(1).ToArray();
                    subCommand.Execute(theRest);
                }

            }
            else if (defaultActionAttribute != null)
            {
                InvokeMethod(defaultActionAttribute.MethodName, args);
            }
            else
            {
                throw new NotSupportedException();
            }

        }

        private void InvokeMethod(string name, string[] args)
        {
            var methodInfo = methods[name];

            var parametersByName = methodInfo.GetParameters().ToDictionary(p => p.Name);
            var parameters = methodInfo.GetParameters().OrderBy(p => p.Position).ToArray();


            var query = from parameter in parameters
                        select GetParameterValue(parameter, args);
            var parameterValues = query.ToArray();
            methodInfo.Invoke(this, parameterValues);
        }

        private object GetParameterValue(ParameterInfo parameter, string[] args)
        {
            var index = Array.IndexOf(args, $"--{parameter.Name}");
            if (index == -1)
                return Type.Missing;

            var next = index + 1;
            if (next < args.Length)
            {
                var value = args[next];
                return value;
            }
            else if (parameter.IsOptional)
            {
                return Type.Missing;
            }
            else
            {

            }
            return null;
        }

        private bool HasParameterValue(ParameterInfo parameter, string[] args)
        {
            return args.Contains($"--{parameter.Name}");
        }

        private Controller GetControllerByName(string name)
        {
            if (SubCommands.ContainsKey(name))
            {
                return SubCommands[name];
            }
            return null;
        }
    }
}