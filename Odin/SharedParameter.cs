using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Odin.Attributes;
using Odin.Conventions;

namespace Odin
{
    /// <summary>
    /// Metadata data for a parameter that is shared across all of the actions of a <see cref="Command">Command</see>
    /// </summary>
    public class SharedParameter : Parameter
    {
        private readonly Command _command;
        private readonly PropertyInfo _propertyInfo;

        /// <summary>
        /// Metadata data for a parameter that is shared across all of the actions of a <see cref="Command">Command</see>
        /// </summary>
        public SharedParameter(Command command, PropertyInfo propertyInfo)
        {
            _command = command;
            _propertyInfo = propertyInfo;
        }

        internal override IConventions Conventions => _command.Conventions;

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public override string Description
        {
            get
            {
                var attr = _propertyInfo.GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description;
            }
        }

        /// <summary>
        /// Gets the default value of the parameter.
        /// </summary>
        /// <remarks>This is always retrieved from the Command property that the parameter refers to.</remarks>
        public override object DefaultValue => _propertyInfo.GetValue(_command);

        /// <summary>
        /// True if the parameter has a default value, otherwise false.
        /// </summary>
        /// <remarks>
        /// Always true in the case of Shared Parameters
        /// </remarks>
        public override bool HasDefaultValue => true;

        /// <summary>
        /// Gets the <see cref="ParserAttribute"/> associated with this parameter.
        /// </summary>
        protected override ParserAttribute ParserAttribute => _propertyInfo.GetCustomAttribute<ParserAttribute>();

        /// <summary>
        /// Gets the Type of the parameter.
        /// </summary>
        public override Type ParameterType => _propertyInfo.PropertyType;

        /// <summary>
        /// Gets the conventional name of the parameter.
        /// </summary>
        public override string Name => _propertyInfo.Name;

        /// <summary>
        /// Gets the <see cref="AliasAttribute"/> associated with this parameter.
        /// </summary>
        protected override AliasAttribute AliasAttribute => _propertyInfo.GetCustomAttribute<AliasAttribute>();

        internal IEnumerable<string> GetNamingConflictForAliases(ActionParameter parameter)
        {
            return this.Aliases.Where(alias => parameter.Aliases.Contains(alias));
        }

        internal bool HasNamingConflict(ActionParameter parameter)
        {
            return this.LongOptionName == parameter.LongOptionName;
        }

        internal void WriteValueToCommand()
        {
            _propertyInfo.SetValue(_command, this.Value);
        }
    }
}