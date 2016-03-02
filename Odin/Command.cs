using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
        /// <summary>
        /// Base class an Odin command.
        /// </summary>
        protected Command()
        {
            _conventions = new HyphenCaseConvention();
            _helpWriter = new DefaultHelpWriter();

            _subCommands = new List<Command>();

            InitializeActions();

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
            return this;
        }

        private void InitializeActions()
        {
            this._actions = this
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ActionAttribute>() != null)
                .Select(row => new MethodInvocation(this, row))
                .ToList()
                ;

            this.CommonParameters  = this.GetType().GetProperties()
                .Where(row => row.GetCustomAttribute<ParameterAttribute>() != null)
                .Select(row => new CommonParameter(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        internal ReadOnlyCollection<CommonParameter> CommonParameters { get; set; }

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
        public virtual ILogger Logger => IsRoot() ? _logger : Parent.Logger;

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

        private List<MethodInvocation> _actions;

        /// <summary>
        /// Gets the actions registered with the current command.
        /// </summary>
        public IReadOnlyCollection<MethodInvocation> Actions => this._actions.AsReadOnly();

        /// <summary>
        /// Adds a command to the command tree.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Command RegisterSubCommand(Command command)
        {
            command.SetParent(this);
            this._subCommands.Add(command);
            return this;
        }

        /// <summary>
        /// Called after arguments are parsed but before the invocation is executed.
        /// </summary>
        /// <param name="invocation"></param>
        protected internal virtual void OnBeforeExecute(MethodInvocation invocation)
        {
        }

        /// <summary>
        /// Called after the invocation is executed.
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="exitCode"></param>
        /// <returns></returns>
        protected internal virtual int OnAfterExecute(MethodInvocation invocation, int exitCode)
        {
            return exitCode;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>0 if successful.</returns>
        public int Execute(params string[] args)
        {
            if (this.DisplayHelpWhenArgsAreEmpty && !args.Any())
            {
                this.Help();
                return 0;
            }

            var result = -1;

            MethodInvocation invocation;
            try
            {
                invocation = this.GenerateInvocation(args);
            }
            catch (UnmappedParameterException)
            {
                this.Logger.Error($"Could not interpret the command. You sent [{args.Join(", ")}]");
                this.Help();
                return result;
            }

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

        /// <summary>
        /// Generates an invocation for the current command.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        /// <remarks>Useful in testing your command structure.</remarks>
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
            return _actions.FirstOrDefault(action => action.Identifiers.Contains(token));
        }

        private Command GetSubCommandByToken(string token)
        {
            return SubCommands.FirstOrDefault(cmd => cmd.IsIdentifiedBy(token));
        }

        /// <summary>
        /// Returns the default action for the command.
        /// </summary>
        /// <returns></returns>
        public MethodInvocation GetDefaultAction()
        {
            return this.Actions.FirstOrDefault(row => row.IsDefault);
        }

        /// <summary>
        /// True if this command is the root of the command tree. Otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool IsRoot()
        {
            return this.Parent == null;
        }

        /// <summary>
        /// Emits help to the logger.
        /// </summary>
        /// <param name="actionName"></param>
        [Action]
        public void Help(
            [Description("The name of the action to provide help for.")]
            string actionName = "")
        {
            var help = this.HelpWriter.Write(this, actionName);
            this.Logger.Info(help);
        }

        /// <summary>
        /// Gets a list of validation messages for the command.
        /// </summary>
        /// <returns></returns>
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
            var defaultActions = this.Actions.Where(row => row.IsDefault).ToArray();
            if (defaultActions.Count() > 1)
            {
                var actionNames = defaultActions.Select(row => row.Name).Join(", ");
                yield return $"There is more than one default action: {actionNames}.";
            }

            var actionIdentifiers = this.Actions.SelectMany(action => action.Identifiers);
            var subCommandIdentifiers = this.SubCommands.SelectMany(cmd => cmd.Identifiers);
            var matchingNames = actionIdentifiers.Intersect(subCommandIdentifiers);

            foreach (var matchingName in matchingNames)
            {
                yield return $"There is more than one executable action named '{matchingName}'.";
            }

            foreach (var action in this._actions)
            {
                var aliases = action.MethodParameters.SelectMany(row => row.Aliases);
                var duplicates = aliases.GroupBy((s) => s).Where(row => row.Count() > 1);
                foreach (var dup in duplicates)
                {
                    yield return $"The alias '{dup.Key}' is duplicated for action '{action.Name}'.";
                }

                foreach (var parameter in action.MethodParameters)
                {
                    foreach (var commonParameter in CommonParameters)
                    {
                        if (commonParameter.LongOptionName == parameter.LongOptionName)
                        {
                            yield return $"The common parameter name '{parameter.LongOptionName}' conflicts with a parameter defined for action '{action.Name}'.";
                        }

                        foreach (var alias in commonParameter.Aliases.Where(alias => parameter.Aliases.Contains(alias)))
                        {
                            yield return
                                $"The alias '{alias}' for common parameter '{commonParameter.LongOptionName}' is duplicated for parameter '{parameter.LongOptionName}' on action '{action.Name}'."
                                ;
                        }
                    }
                }
            }

            var commonParameterAliases = this.CommonParameters.SelectMany(row => row.Aliases);
            var commonParameterDuplicates = commonParameterAliases.GroupBy((s) => s).Where(row => row.Count() > 1);
            foreach (var dup in commonParameterDuplicates)
            {
                yield return $"The alias '{dup.Key}' is duplicated amongst common parameters.";
            }

        }
    }
}