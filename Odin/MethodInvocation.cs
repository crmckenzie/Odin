using System;
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
    public class MethodInvocation
    {
        public MethodInvocation(Command command, MethodInfo methodInfo)
        {
            Command = command;
            MethodInfo = methodInfo;

            IsDefault = methodInfo.GetCustomAttribute<ActionAttribute>().IsDefault;

            ParameterValues = MethodInfo.GetParameters()
                .Select(row => new ParameterValue(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        public bool IsDefault { get; }

        public MethodInfo MethodInfo { get; }

        public Command Command { get; }
        public IConventions Conventions => Command.Conventions;
        public string Name => Conventions.GetActionName(MethodInfo);
        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }

        public string[] Aliases
        {
            get { return MethodInfo.GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] {}; }
        }

        public string[] Identifiers
        {
            get { return Aliases.Concat(new string[] {this.Name}).ToArray(); }
        }

        public bool IsIdentifiedBy(string token)
        {
            return Name == token || Aliases.Contains(token);
        }


        private ParameterValue FindByToken(string token)
        {
            return ParameterValues
                .FirstOrDefault(p => p.IsIdentifiedBy(token))
                ;
        }

        private ParameterValue FindByIndex(int i)
        {
            if (i >= ParameterValues.Count)
                return null;
            return ParameterValues
                .OrderBy(p => p.Position)
                .ToArray()[i]
                ;
        }

        public bool CanInvoke()
        {
            return ParameterValues.All(row => row.IsValueSet());
        }

        public int Invoke()
        {
            var args = ParameterValues
                .OrderBy(map => map.ParameterInfo.Position)
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

        private ParameterValue FindParameter(string token, int i)
        {
            var parameter = FindByToken(token);
            if (parameter != null) return parameter;

            if (Conventions.IsParameterName(token)) return null;
            return FindByIndex(i);
        }

        private IParser CreateParser(ParameterValue parameter)
        {
            return parameter.HasCustomParser() ? CreateCustomParser(parameter) : Conventions.CreateParser(parameter);
        }

        private IParser CreateCustomParser(ParameterValue parameter)
        {
            var parserAttribute = parameter.ParameterInfo.GetCustomAttribute<ParserAttribute>();
            if (!parserAttribute.ParserType.Implements<IParser>())
                throw new ArgumentOutOfRangeException(
                    $"'{parserAttribute.ParserType.FullName}' is not an implementation of '{typeof (IParser).FullName}.'");

            var types = new[] {typeof (ParameterValue)};
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