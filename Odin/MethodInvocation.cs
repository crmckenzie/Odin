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
using Odin.Parsing;

namespace Odin
{
    public class MethodInvocation
    {
        public MethodInvocation(Command command, MethodInfo methodInfo)
        {
            this.Command = command;
            this.MethodInfo = methodInfo;

            IsDefault = methodInfo.GetCustomAttribute<ActionAttribute>().IsDefault;

            this.ParameterValues = this.MethodInfo.GetParameters()
                .Select(row => new ParameterValue(this, row))
                .ToList()
                .AsReadOnly()
                ;
        }

        public bool IsDefault { get; }

        private MethodInfo MethodInfo { get; }

        public Command Command { get; }
        public Conventions Conventions => Command.Conventions;
        public string Name => Conventions.GetActionName(MethodInfo);
        public ReadOnlyCollection<ParameterValue> ParameterValues { get; }

        public string[] Aliases
        {
            get { return this.MethodInfo.GetCustomAttribute<AliasAttribute>()?.Aliases.ToArray() ?? new string[] { }; }
        }

        public bool IsIdentifiedBy(string token)
        {
            return Name == token || Aliases.Contains(token);
        }

        public string GetDescription()
        {
            var descriptionAttr = MethodInfo.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttr == null ? "" : descriptionAttr.Description;
        }

        private ParameterValue FindByToken(string token)
        {
            //return this.ParameterValues.FirstOrDefault(p => p.Identifiers.Contains(token));

            return this.ParameterValues
                .FirstOrDefault(p => p.IsIdentifiedBy(token))
                ;
        }

        private ParameterValue FindByIndex(int i)
        {
            if (i >= this.ParameterValues.Count)
                return null;
            return  this.ParameterValues
                .OrderBy(p => p.Position)
                .ToArray()[i]
                ;
        }

        public bool CanInvoke()
        {
            return this.ParameterValues.All(row => row.IsValueSet()) ;
        }

        public int Invoke()
        {
            var args = this.ParameterValues
                .OrderBy(map => map.ParameterInfo.Position)
                .Select(row => row.Value)
                .ToArray()
                ;

            var result = this.MethodInfo.Invoke(this.Command, args);
            if (result is int)
            {
                return (int)result;
            }

            if (result is bool)
            {
                return ((bool) result) ? 0 : -1;
            }

            return 0;
        }

        internal void SetParameterValues(string[] tokens)
        {
            var i = 0;
            while (i < tokens.Length)
            {
                var token = tokens[i];
                var parameter = FindByToken(token) ?? FindByIndex(i);
                if (parameter == null)
                {
                    i++;
                    continue;
                };
                ParseResult result;

                try
                {
                    var parser = CreateParser(parameter);
                    result = parser.Parse(tokens, i);
                    if (result.TokensProcessed <= 0)
                    {
                        i++;
                        continue;
                    };

                    parameter.Value = result.Value;
                }
                catch (Exception e)
                {
                    throw new ParameterConversionException(parameter, tokens[i], e);
                }
                i += result.TokensProcessed;

            }

        }

        private IParser CreateParser(ParameterValue parameter)
        {
            return parameter.HasCustomParser() ? CreateCustomParser(parameter) : Conventions.CreateParser(parameter);
        }

        private IParser CreateCustomParser(ParameterValue parameter)
        {
            var parserAttribute = parameter.ParameterInfo.GetCustomAttribute<ParserAttribute>();
            if (!parserAttribute.ParserType.Implements<IParser>())
                throw new ArgumentOutOfRangeException($"'{parserAttribute.ParserType.FullName}' is not an implementation of '{typeof(IParser).FullName}.'");

            var types = new[] { typeof(ParameterValue) };
            var constructor = parserAttribute.ParserType.GetConstructor(types);
            if (constructor == null)
            {
                throw new TypeInitializationException(
                    "Could not find a constructor with the signature (ParameterValue).", null);
            }

            var parameters = new[] { parameter };
            var instance = constructor.Invoke(parameters);
            var typedInstance = (IParser)instance;
            return typedInstance;
        }

    }
}