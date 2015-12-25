using System;
using System.Reflection;
using Odin.Attributes;
using Odin.Parsing;

namespace Odin.Configuration
{
    public abstract class Conventions
    {
        public abstract string GetCommandName(Command command);
        public abstract string GetActionName(string methodName);
        public abstract string GetLongOptionName(string parameterName);

        public abstract string GetNegatedLongOptionName(string parameterName);

        public abstract string GetShortOptionName(string alias);

        public abstract bool IsMatchingParameterName(string parameterName, string token);

        public abstract IParser CreateParser(ParameterValue parameter);

    }
}