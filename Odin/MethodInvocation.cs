using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Configuration;
using Odin.Exceptions;
using Odin.Parsing;

namespace Odin
{
    /// <summary>
    /// Represents an action to be executed.
    /// </summary>
    public class MethodInvocation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="methodInfo"></param>
        public MethodInvocation(Command command, MethodInfo methodInfo)
        {
            Command = command;
            MethodInfo = methodInfo;

            IsDefault = methodInfo.GetCustomAttribute<ActionAttribute>().IsDefault;

            MethodParameters = MethodInfo
                .GetParameters()
                .Select(row => new MethodParameter(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        /// <summary>
        /// Gets the list of <see cref="CommonParameter"/>'s available to the MethodInvocation.
        /// </summary>
        public ReadOnlyCollection<CommonParameter> CommonParameters => Command.CommonParameters;

        /// <summary>
        /// Gets whether or not this action is the default one for its Command.
        /// </summary>
        public bool IsDefault { get; }

        /// <summary>
        /// Gets the MethodInfo wrapped by the action.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the command the action is tied to.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        /// Gets the conventions used in the command tree.
        /// </summary>
        public IConventions Conventions => Command.Conventions;

        /// <summary>
        /// Gets the conventional name of the action.
        /// </summary>
        public string Name => Conventions.GetActionName(MethodInfo);

        /// <summary>
        /// Gets the collection of <see cref="MethodParameter"/>'s available to the MethodInvocation.
        /// </summary>
        public ReadOnlyCollection<MethodParameter> MethodParameters { get; }

        /// <summary>
        /// Gets the list of aliases applied to the action.
        /// </summary>
        public string[] Aliases
        {
            get { return MethodInfo.GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] {}; }
        }

        /// <summary>
        /// Gets the list of identifiers representing the action.
        /// </summary>
        public string[] Identifiers
        {
            get { return Aliases.Concat(new string[] {this.Name}).ToArray(); }
        }

        private Parameter FindByToken(string token)
        {
            Parameter result = MethodParameters
                .FirstOrDefault(p => p.IsIdentifiedBy(token))
                ;

            if (result == null)
            {
                result = CommonParameters.FirstOrDefault(p => p.IsIdentifiedBy(token));
            }
            return result;
        }

        private MethodParameter FindByIndex(int i)
        {
            if (i >= MethodParameters.Count)
                return null;
            return MethodParameters
                .OrderBy(p => p.Position)
                .ToArray()[i]
                ;
        }

        /// <summary>
        /// True if the action is invokable. Otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool CanInvoke()
        {
            return MethodParameters.All(row => row.IsValueSet());
        }

        /// <summary>
        /// Invokes the action with the parsed parameters.
        /// </summary>
        /// <returns>0 for success.</returns>
        public int Invoke()
        {
            var args = MethodParameters
                .OrderBy(map => map.Position)
                .Select(row => row.Value)
                .ToArray()
                ;

            var result = MethodInfo.Invoke(Command, args);
            if (result is int)
            {
                return (int) result;
            }

            if (result is bool)
            {
                return (bool) result ? 0 : -1;
            }

            return 0;
        }

        internal void SetParameterValues(string[] tokens)
        {
            var i = 0;
            while (i < tokens.Length)
            {
                var token = tokens[i];
                var parameter = FindParameter(token, i);
                if (parameter == null)
                    throw new UnmappedParameterException($"Unable to map parameter '{token}' to action '{Name}'");

                try
                {
                    var parser = CreateParser(parameter);
                    var result = parser.Parse(tokens, i);
                    if (result.TokensProcessed <= 0)
                    {
                        i++;
                        continue;
                    }

                    parameter.Value = result.Value;
                    i += result.TokensProcessed;
                }
                catch (UnmappedParameterException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new ParameterConversionException(parameter, tokens[i], e);
                }
            }
        }

        private Parameter FindParameter(string token, int i)
        {
            var parameter = FindByToken(token);
            if (parameter != null) return parameter;

            if (Conventions.IsParameterName(token)) return null;
            return FindByIndex(i);
        }

        private IParser CreateParser(Parameter parameter)
        {
            return parameter.HasCustomParser() ? CreateCustomParser(parameter) : Conventions.CreateParser(parameter);
        }

        private IParser CreateCustomParser(Parameter parameter)
        {
            var parserAttribute = parameter.ParserAttribute;
            if (!parserAttribute.ParserType.Implements<IParser>())
                throw new ArgumentOutOfRangeException(
                    $"'{parserAttribute.ParserType.FullName}' is not an implementation of '{typeof (IParser).FullName}.'");

            var types = new[] {typeof (Parameter)};
            var constructor = parserAttribute.ParserType.GetConstructor(types);
            if (constructor == null)
            {
                throw new TypeInitializationException(
                    "Could not find a constructor with the signature (ParameterValue).", null);
            }

            var parameters = new[] {parameter};
            var instance = constructor.Invoke(parameters);
            var typedInstance = (IParser) instance;
            return typedInstance;
        }
    }
}