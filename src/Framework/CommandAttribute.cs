using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Provides metadata describing a concrete <see cref="Command"/> class.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandAttribute : ExportAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        public CommandAttribute() : base(typeof(Command))
        {
        }
    }
}
