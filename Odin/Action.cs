using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Conventions;
using Odin.Exceptions;
using Odin.Parsing;

namespace Odin
{
    /// <summary>
    /// Represents an action to be executed.
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="methodInfo"></param>
        public Action(Command command, MethodInfo methodInfo)
        {
            Command = command;
            MethodInfo = methodInfo;

            IsDefault = methodInfo.GetCustomAttribute<ActionAttribute>().IsDefault;

            Parameters = MethodInfo
                .GetParameters()
                .Select(row => new ActionParameter(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }
        /// <summary>
        /// Gets whether or not this action is the default one for its Command.
        /// </summary>
        public bool IsDefault { get; }

        /// <summary>
        /// Gets the MethodInfo wrapped by the action.
        /// </summary>
        private MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the command the action is tied to.
        /// </summary>
        public Command Command { get; }

        public string GetDescription()
        {
            var descriptionAttr = MethodInfo.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttr?.Description ?? "";
        }

        /// <summary>
        /// Gets the conventions used in the command tree.
        /// </summary>
        internal IConventions Conventions => Command.Conventions;

        /// <summary>
        /// Gets the conventional name of the action.
        /// </summary>
        public string Name => Conventions.GetActionName(MethodInfo);

        /// <summary>
        /// Gets the collection of <see cref="ActionParameter"/>'s available to the Action.
        /// </summary>
        public ReadOnlyCollection<ActionParameter> Parameters { get; }

        /// <summary>
        /// Gets the list of aliases applied to the action.
        /// </summary>
        public string[] Aliases => MethodInfo.GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] {};

        /// <summary>
        /// Gets the list of identifiers representing the action.
        /// </summary>
        public string[] Identifiers => Aliases.Concat(new[] {this.Name}).ToArray();

        /// <summary>
        /// Gets the list of <see cref="SharedParameter"/>'s available to the Action.
        /// </summary>
        private ReadOnlyCollection<SharedParameter> SharedParameters => Command.SharedParameters;

        private Parameter FindByToken(string token)
        {
            var result = Parameters.Cast<Parameter>()
                .FirstOrDefault(p => p.IsIdentifiedBy(token))
                ;

            if (result != null)
                return result;

            return SharedParameters.FirstOrDefault(p => p.IsIdentifiedBy(token));
        }

        private ActionParameter FindByIndex(int i)
        {
            if (i >= Parameters.Count)
                return null;
            return Parameters
                .OrderBy(p => p.Position)
                .ToArray()[i]
                ;
        }

        /// <summary>
        /// True if the action is invokable. Otherwise false.
        /// </summary>
        /// <returns></returns>
        private bool AllParametersHaveAValue()
        {
            return Parameters.All(row => row.IsValueSet());
        }

        /// <summary>
        /// Invokes the action with the parsed parameters.
        /// </summary>
        /// <returns>0 for success.</returns>
        internal int Invoke()
        {
            if (!AllParametersHaveAValue())
            {
                throw new ParameterMisMatchException($"Unable to supply required parameters to \n");
            }

            this.SharedParameters.ToList().ForEach(cp => cp.WriteValueToCommand());
            this.Command.OnBeforeExecute(this);

            var exitCode = InvokeMethod();

            return this.Command.OnAfterExecute(this, exitCode);
        }

        private static int ConvertToExitCode(object result)
        {
            switch (result)
            {
                case int i:
                    return i;
                case bool _:
                    return (bool) result ? 0 : -1;
                default:
                    return 0;
            }
        }

        private int InvokeMethod()
        {
            var args = Parameters
                    .OrderBy(map => map.Position)
                    .Select(row => row.Value)
                    .ToArray()
                ;
            var rawResult = MethodInfo.Invoke(Command, args);
            return ConvertToExitCode(rawResult);
        }

        internal void SetParameterValues(string[] tokens)
        {
            var index = 0;
            while (index < tokens.Length)
            {
                var token = tokens[index];
                var parameter = FindParameter(token, index);
                var result = parameter.Parse(tokens, index);
                parameter.Value = result.Value;
                index += result.TokensProcessed;
            }
        }

        private Parameter FindParameter(string token, int index)
        {
            var parameter = FindByToken(token);
            if (parameter != null) return parameter;

            parameter =  Conventions.IsParameterName(token) ? null : FindByIndex(index);

            if (parameter == null)
                throw new UnmappedParameterException(token, Name);

            return parameter;
        }

        internal IEnumerable<IGrouping<string, string>> GetDuplicateAliases()
        {
            var aliases = this.Parameters.SelectMany(row => row.Aliases);
            var duplicates = aliases.GroupBy((s) => s).Where(row => row.Count() > 1);
            return duplicates;
        }

        internal IEnumerable<string> ValidateAliases()
        {
            var aliases = this.GetDuplicateAliases();
            var validationMessages = aliases.Select(dup => $"The alias '{dup.Key}' is duplicated for action '{this.Name}'.");
            return validationMessages;
        }

        internal IEnumerable<ParameterAnalysis> GetParameterAnalysis()
        {
            var query = from parameter in Parameters
                from shared in SharedParameters
                select new ParameterAnalysis
                {
                    Action = this,
                    Parameter = parameter,
                    SharedParameter = shared,
                    HasNamingConflict = shared.HasNamingConflict(parameter),
                    NamingConflictsForAliases = shared.GetNamingConflictForAliases(parameter)
                };

            return query;
        }

        internal IEnumerable<string> GetValidationMessages()
        {
            var results = ValidateAliases().ToList();
            foreach (var row in GetParameterAnalysis())
            {
                if (row.HasNamingConflict)
                    results.Add($"The shared parameter name '{row.Parameter.LongOptionName}' conflicts with a parameter defined for action '{Name}'.");
                results.AddRange(row.GetNamingConflictsMessages());
            }

            return results;
        }
    }
}