using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Odin.Attributes;
using Odin.Conventions;
using Odin.Parsing;

namespace Odin
{
    /// <summary>
    /// Represents an action parameter and its value.
    /// </summary>C:\Users\chmckenz\src\odin\Odin\ActionParameter.cs
    public class ActionParameter : Parameter
    {
        private readonly ParameterInfo _parameterInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameterInfo"></param>
        public ActionParameter(Action action, ParameterInfo parameterInfo)
        {
            Action = action;
            _parameterInfo = parameterInfo;

            ConfigureDefaultValue();
        }
        private Action Action { get; }

        public override IConventions Conventions => Action.Conventions;

        /// <summary>
        /// Gets the description of the Parameter
        /// </summary>
        public override string Description
        {
            get
            {
                var attr = _parameterInfo.GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description;
            }
        }

        /// <summary>
        /// Gets the default value for the Parameter
        /// </summary>
        public override object DefaultValue => _parameterInfo.DefaultValue;

        /// <summary>
        /// True if the Parameter has a default value, otherwise false.
        /// </summary>
        public override bool HasDefaultValue => _parameterInfo.HasDefaultValue;

        /// <summary>
        /// Gets the <see cref="ParserAttribute"/> associated with the Parameter
        /// </summary>
        public override ParserAttribute ParserAttribute => _parameterInfo.GetCustomAttribute<ParserAttribute>();

        /// <summary>
        /// Gets the <see cref="AliasAttribute"/> associated with the Parameter
        /// </summary>
        protected override AliasAttribute AliasAttribute =>  _parameterInfo.GetCustomAttribute<AliasAttribute>();

        private bool IsOptional => _parameterInfo.IsOptional;

        private void ConfigureDefaultValue()
        {
            if (ParameterType == typeof (bool))
                Value = false;

            if (ParameterType.IsNullableType())
                Value = null;

            if (IsOptional)
                Value = Type.Missing;
        }

        /// <summary>
        /// Gets the Type of the parameter.
        /// </summary>
        public override Type ParameterType => _parameterInfo.ParameterType;

        /// <summary>
        /// Gets the position of the parameter.
        /// </summary>
        public int Position => _parameterInfo.Position;

        /// <summary>
        /// Gets the conventional name of the parameter.
        /// </summary>
        public override string Name => _parameterInfo.Name;
    }
}