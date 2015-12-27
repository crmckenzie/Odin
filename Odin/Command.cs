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
        protected Command()
        {
            _conventions = new HyphenCaseConvention();
            _helpWriter = new DefaultHelpWriter();

            _subCommands = new Dictionary<string, Command>();
            SubCommands =new ReadOnlyDictionary<string, Command>(_subCommands);

            ReKeyActions();

            this.Description = GetDescription();
            this.DisplayHelpWhenArgsAreEmpty = true;
        }

        public bool DisplayHelpWhenArgsAreEmpty { get; set; }

        public Command Use(Conventions conventions)
        {
            _conventions = conventions;
            ReKeySubCommands();
            ReKeyActions();
            return this;
        }

        private void ReKeyActions()
        {
            this._actions = this
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ActionAttribute>() != null)
                .Select(row => new MethodInvocation(this, row))
                .ToList().ToDictionary(action => action.Name)
                ;
            this.Actions = new ReadOnlyDictionary<string, MethodInvocation>(this._actions);
        }

        private void ReKeySubCommands()
        {
            this._subCommands = this._subCommands.Values.ToDictionary(row => row.Name);
            SubCommands = new ReadOnlyDictionary<string, Command>(_subCommands);
        }

        public Command Use(Logger logger)
        {
            _logger = logger ?? new DefaultLogger();
            return this;
        }

        public Command Use(IHelpWriter helpWriter)
        {
            _helpWriter = helpWriter;
            return this;
        }

        public virtual string Description { get; }

        private void SetParent(Command parent)
        {
            this.Parent = parent;
        }

        public Command Parent { get; set; }

        private Logger _logger = new DefaultLogger();
        public Logger Logger => IsRoot() ? _logger : Parent.Logger;

        private  Conventions _conventions;
        private IHelpWriter _helpWriter;

        public IHelpWriter HelpWriter => IsRoot() ? _helpWriter : Parent.HelpWriter;

        public Conventions Conventions => IsRoot() ? _conventions : Parent.Conventions ;

        public virtual string Name => Conventions.GetCommandName(this);

        public virtual string[] Aliases
        {
            get { return this.GetType().GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] { }; }
        }

        public virtual bool IsIdentifiedBy(string token)
        {
            return token == Name || Aliases.Contains(token);
        }

        private Dictionary<string, Command> _subCommands;
        public IReadOnlyDictionary<string, Command> SubCommands { get; private set; }

        private Dictionary<string, MethodInvocation> _actions;

        public IReadOnlyDictionary<string, MethodInvocation> Actions { get; private set; }

        protected virtual string GetDescription()
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
            if (this.DisplayHelpWhenArgsAreEmpty && !args.Any())
            {
                this.Help();
                return 0;
            }

            var result = -1;
            var invocation = this.GenerateInvocation(args);
            if (invocation?.CanInvoke() == true)
                result =  invocation.Invoke();

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
                var token = tokens.FirstOrDefault();
                var subCommand = GetSubCommandByToken(token);
                if (subCommand != null)
                {
                    var theRest = tokens.Skip(1).ToArray();
                    return subCommand.GenerateInvocation(theRest);
                }

                var action = GetActionByToken(token);
                var toSkip = 1;
                if (action == null)
                {
                    toSkip = 0;
                    action = GetDefaultAction();
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

        private MethodInvocation GetActionByToken(string token)
        {
            return _actions.Values.FirstOrDefault(action => action.IsIdentifiedBy(token));
        }

        private Command GetSubCommandByToken(string token)
        {
            return SubCommands.Values.FirstOrDefault(cmd => cmd.IsIdentifiedBy(token));
        }

        private MethodInvocation GetDefaultAction()
        {
            return this.Actions.Values.FirstOrDefault(row => row.IsDefault);
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
            var help = this.HelpWriter.Write(this, actionName);
            this.Logger.Info(help);
        }
    }
}