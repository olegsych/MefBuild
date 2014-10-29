using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Indicates that a property or a parameter marked with the <see cref="ImportAttribute"/> can
    /// be imported from a command-line argument.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether parameter or property to which this attribute is applied
        /// can be imported from a command-line argument.
        /// </summary>
        public static bool IsArgument 
        { 
            get { return true; } 
        }
    }
}
