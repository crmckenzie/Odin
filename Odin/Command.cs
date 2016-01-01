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
    /// <summary>
    /// Base class an Odin command.
    /// </summary>
    /// <remarks>
    /// Commands are organized in a tree structure to indicate subcomands.
    /// Actions on commands are callable at their level in tree structure.
    /// SubCommands, Actions, and Parameters are resolved from the input tokens by convention.
    /// </remarks>
    public abstract class Command
    {
        protected Command()
        {
            _conventions = new HyphenCaseConvention();
            _helpWriter = new DefaultHelpWriter();

            _subCommands = new List<Command>();

            ReKeyActions();

            this.DisplayHelpWhenArgsAreEmpty = true;
        }

        /// <summary>
        /// Gets or sets if the command should emit help when args are empty.
        /// </summary>
        public bool DisplayHelpWhenArgsAreEmpty { get; set; }

        /// <summary>
        /// Sets the convention to be used with the command. Only conventions applied to the root command are used.
        /// </summary>
        /// <param name="conventions"></param>
        /// <returns></returns>
        public Command Use(IConventions conventions)
        {
            _conventions = conventions;
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

        /// <summary>
        /// Sets the logger to be used with the command. Only the logger applied to the root command is used.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public Command Use(ILogger logger)
        {
            _logger = logger ?? new ConsoleLogger();
            return this;
        }

        /// <summary>
        /// Sets the helpwriter to be used with the command. Only the helpwriter applied to the root command is used.
        /// </summary>
        /// <param name="helpWriter"></param>
        /// <returns></returns>
        public Command Use(IHelpWriter helpWriter)
        {
            _helpWriter = helpWriter;
            return this;
        }

        private void SetParent(Command parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the parent of the command.
        /// </summary>
        public Command Parent { get; private set; }

        private ILogger _logger = new ConsoleLogger();
        /// <summary>
        /// Gets the logger for the command tree.
        /// </summary>
        public ILogger Logger => IsRoot() ? _logger : Parent.Logger;

        private  IConventions _conventions;
        private IHelpWriter _helpWriter;

        /// <summary>
        /// Gets the helpwriter for the command tree.
        /// </summary>
        public IHelpWriter HelpWriter => IsRoot() ? _helpWriter : Parent.HelpWriter;

        /// <summary>
        /// Gets the conventions for the command tree.
        /// </summary>
        public IConventions Conventions => IsRoot() ? _conventions : Parent.Conventions ;

        /// <summary>
        /// Gets the conventional name of the command.
        /// </summary>
        public virtual string Name => Conventions.GetCommandName(this);

        /// <summary>
        /// Gets the aliases applied to the command.
        /// </summary>
        public virtual string[] Aliases
        {
            get { return this.GetType().GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] { }; }
        }

        /// <summary>
        /// Gets all of the identifiers applied to the command.
        /// </summary>
        public string[] Identifiers
        {
            get { return Aliases.Concat(new string[] {this.Name}).ToArray(); }
        }

        /// <summary>
        /// True if the token matches the command, otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual bool IsIdentifiedBy(string token)
        {
            return token == Name || Aliases.Contains(token);
        }

        private List<Command> _subCommands;

        /// <summary>
        /// Gets the subcommands registered with the current command.
        /// </summary>
        public IReadOnlyCollection<Command> SubCommands => this._subCommands.AsReadOnly();

        private Dictionary<string, MethodInvocation> _actions;

        public IReadOnlyDictionary<string, MethodInvocation> Actions { get; private set; }

        public Command RegisterSubCommand(Command command)
        {
            command.SetParent(this);
            this._subCommands.Add(command);
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
            return SubCommands.FirstOrDefault(cmd => cmd.IsIdentifiedBy(token));
        }

        public MethodInvocation GetDefaultAction()
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

        public IEnumerable<ValidationResult> Validate()
        {
            var messages = GetValidationMessages().ToArray();
            if (messages.Any())
                yield return new ValidationResult(this.Name, messages);


            var validationResults = this.SubCommands.SelectMany(cmd => cmd.Validate());
            foreach (var validationResult in validationResults)
            {
                yield return validationResult;
            }
        }

        private IEnumerable<string> GetValidationMessages()
        {
            var defaultActions = this.Actions.Values.Where(row => row.IsDefault).ToArray();
            if (defaultActions.Count() > 1)
            {
                var actionNames = defaultActions.Select(row => row.Name).Join(", ");
                yield return $"There is more than one default action: {actionNames}.";
            }

            var actionIdentifiers = this.Actions.Values.SelectMany(action => action.Identifiers);
            var subCommandIdentifiers = this.SubCommands.SelectMany(cmd => cmd.Identifiers);
            var matchingNames = actionIdentifiers.Intersect(subCommandIdentifiers);

            foreach (var matchingName in matchingNames)
            {
                yield return $"There is more than one executable action named '{matchingName}'.";
            }
        }
    }
}