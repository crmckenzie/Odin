using System;
using System.Collections.Generic;
using System.Linq;

namespace Odin
{
    public class EntryPoint
    {
        private readonly Controller[] _controllers;
        private Controller defaultController;
        private Dictionary<string, Controller> controllersByName;

        public EntryPoint(params Controller[] controllers)
        {
            _controllers = controllers;
            this.defaultController = _controllers.First();
            this.controllersByName = _controllers.ToDictionary(row => row.Name);
        }

        public void Execute(string[] args)
        {
            var controllerName = args.Any() ? args.First() : "";

            var controller = GetControllerByName(controllerName);
            if (controller == null)
            {
                defaultController.Execute(args);
            }
            else
            {
                controller.Execute(args.Skip(1).ToArray());
            }

        }

        private Controller GetControllerByName(string name)
        {
            if (controllersByName.ContainsKey(name))
            {
                return controllersByName[name];
            }
            return null;
        }
    }
}