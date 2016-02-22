using System;

namespace Odin.Attributes
{
    /// <summary>
    /// Identifies a parameter shared across multiple actions on a <see cref="Command"></see>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : System.Attribute
    {
    }
}