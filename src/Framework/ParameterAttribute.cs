using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Provides metadata describing an input parameter of a command.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name { get; set; }
    }
}
