using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Odin.Attributes;
using Odin.Conventions;
using Odin.Exceptions;
using Odin.Help;
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
            _conventions = new KebabCaseConvention();
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
                .Select(row => new Action(this, row))
                .ToList()
                ;

            this.SharedParameters = this.GetType().GetProperties()
                .Where(row => row.GetCustomAttribute<ParameterAttribute>() != null)
                .Select(row => new SharedParameter(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        internal ReadOnlyCollection<SharedParameter> SharedParameters { get; set; }

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

        private IConventions _conventions;
        private IHelpWriter _helpWriter;

        /// <summary>
        /// Gets the helpwriter for the command tree.
        /// </summary>
        public IHelpWriter HelpWriter => IsRoot() ? _helpWriter : Parent.HelpWriter;

        /// <summary>
        /// Gets the conventions for the command tree.
        /// </summary>
        public IConventions Conventions => IsRoot() ? _conventions : Parent.Conventions;

        /// <summary>
        /// Gets the conventional name of the command.
        /// </summary>
        public virtual string Name => Conventions.GetCommandName(this);

        /// <summary>
        /// Gets the aliases applied to the command.
        /// </summary>
        public virtual string[] Aliases => GetType().GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] { };

        /// <summary>
        /// Gets all of the identifiers applied to the command.
        /// </summary>
        public string[] Identifiers => Aliases.Concat(new string[] { this.Name }).ToArray();

        /// <summary>
        /// True if the token matches the command, otherwise false.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual bool IsIdentifiedBy(string token)
        {
            return token == Name || Aliases.Contains(token);
        }

        private readonly List<Command> _subCommands;

        /// <summary>
        /// Gets the subcommands registered with the current command.
        /// </summary>
        public IReadOnlyCollection<Command> SubCommands => this._subCommands.AsReadOnly();

        private List<Action> _actions;

        /// <summary>
        /// Gets the actions registered with the current command.
        /// </summary>
        public IReadOnlyCollection<Action> Actions => this._actions.AsReadOnly();

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
        protected internal virtual void OnBeforeExecute(Action invocation)
        {
        }

        /// <summary>
        /// Called after the invocation is executed.
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="exitCode"></param>
        /// <returns></returns>
        protected internal virtual int OnAfterExecute(Action invocation, int exitCode)
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
            if (ShouldExit(args))
                return Exit(displayHelp: true);

            const int commandFailed = -1;
            const int commandSucceeded = 0;

            var exitCode = commandFailed;

            Action action;
            try
            {
                action = this.GetAction(args);
            }
            catch (UnmappedParameterException)
            {
                this.Logger.Error($"Could not interpret the command. You sent [{args.Join(", ")}]");
                this.Help();
                return exitCode;
            }

            if (action?.CanInvoke() == true)
            {
                exitCode = action.Invoke();
                if (exitCode == commandSucceeded)
                {
                    return commandSucceeded;
                }
            }

            this.Logger.Error("Unrecognized command sequence: {0}\n", string.Join(" ", args));
            this.Help();
            return exitCode;
        }

        private int Exit(bool displayHelp = false)
        {
            if (displayHelp)
                this.Help();
            return 0;
        }

        private bool ShouldExit(string[] args)
        {
            return this.DisplayHelpWhenArgsAreEmpty && !args.Any();
        }

        /// <summary>
        /// Generates an invocation for the current command.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        /// <remarks>Useful in testing your command structure.</remarks>
        public Action GetAction(params string[] tokens)
        {
            try
            {
                var token = tokens.FirstOrDefault();
                var invocation = GetSubCommandByToken(tokens, token);
                return invocation.Found ? invocation.SubCommand : GetAction(tokens, token);
            }
            catch (ParameterConversionException pce)
            {
                this.Logger.Error(pce.Message);
                return null;
            }
        }

        private Action GetAction(string[] tokens, string token)
        {
            var action = GetActionByToken(token);
            var tokensProcessed = action == null ? 0 : 1;
            action = action ?? GetDefaultAction();

            var args = tokens.Skip(tokensProcessed).ToArray();
            action?.SetParameterValues(args);
            return action;
        }

        private (bool Found, Action SubCommand) GetSubCommandByToken(string[] tokens, string token)
        {
            var subCommand = SubCommands.FirstOrDefault(cmd => cmd.IsIdentifiedBy(token));
            if (subCommand == null) return (Found: false, SubCommand: null);

            var theRest = tokens.Skip(1).ToArray();
            return (Found: true, SubCommand: subCommand.GetAction(theRest));
        }

        private Action GetActionByToken(string token)
        {
            return _actions.FirstOrDefault(action => action.Identifiers.Contains(token));
        }

        /// <summary>
        /// Returns the default action for the command.
        /// </summary>
        /// <returns></returns>
        public Action GetDefaultAction()
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
            var validationErrors = new List<string>();

            var defaultActionResults = ValidateDefaultActions();
            validationErrors.AddRange(defaultActionResults);

            var executableActionResults = ValidateExecutableActions();
            validationErrors.AddRange(executableActionResults);

            var actionResults = ValidateActions();
            validationErrors.AddRange(actionResults);

            var commonParameterResults = ValidateSharedParameters();
            validationErrors.AddRange(commonParameterResults);

            return validationErrors;
        }

        private IEnumerable<string> ValidateDefaultActions()
        {
            var defaultActions = this.Actions.Where(row => row.IsDefault).ToArray();
            if (defaultActions.Count() <= 1)
                return Enumerable.Empty<string>();

            var actionNames = defaultActions
                .Select(row => row.Name)
                .Join(", ")
                ;

            return new[]
            {
                $"There is more than one default action: {actionNames}."
            };
        }

        private IEnumerable<string> ValidateExecutableActions()
        {
            var actions = GetDuplicateActionNames();
            var messages = actions.Select(matchingName => $"There is more than one executable action named '{matchingName}'.");
            return messages;
        }

        private IEnumerable<string> ValidateSharedParameters()
        {
            var aliases = GetDuplicatedSharedParameterAliases();
            var validationResults = aliases.Select(item => $"The alias '{item.Key}' is duplicated amongst shared parameters.");
            return validationResults;
        }

        private IEnumerable<string> ValidateActions()
        {
            var results = new List<string>();
            foreach (var action in this._actions)
            {
                var validationMessages = action.ValidateAliases();
                results.AddRange(validationMessages);

                foreach (var parameter in action.Parameters)
                {
                    foreach (var sharedParameter in SharedParameters)
                    {
                        if (sharedParameter.HasNamingConflict(parameter))
                        {
                            results.Add($"The shared parameter name '{parameter.LongOptionName}' conflicts with a parameter defined for action '{action.Name}'.");
                        }

                        var conflicts = sharedParameter.GetNamingConflictForAliases(parameter)
                            .Select(alias =>
                                $"The alias '{alias}' for shared parameter '{sharedParameter.LongOptionName}' is duplicated for parameter '{parameter.LongOptionName}' on action '{action.Name}'.");
                        results.AddRange(conflicts);
                    }
                }
            }

            return results;
        }

        private IEnumerable<IGrouping<string, string>> GetDuplicatedSharedParameterAliases()
        {
            var aliases = this.SharedParameters.SelectMany(row => row.Aliases);
            var duplicates = aliases.GroupBy((s) => s).Where(row => row.Count() > 1);
            return duplicates;
        }

        private IEnumerable<string> GetDuplicateActionNames()
        {
            var actionIdentifiers = this.Actions.SelectMany(action => action.Identifiers);
            var subCommandIdentifiers = this.SubCommands.SelectMany(cmd => cmd.Identifiers);
            var matchingNames = actionIdentifiers.Intersect(subCommandIdentifiers);
            return matchingNames;
        }
    }
}