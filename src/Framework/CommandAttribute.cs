using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

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

        /// <summary>
        /// Gets or sets a collection of types derived from the <see cref="Command"/> class after which 
        /// the command marked with the <see cref="CommandAttribute"/> must be executed.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Collections are not valid attribute parameter types")]
        public Type[] ExecuteAfter { get; set; }

        /// <summary>
        /// Gets or sets a collections of types derived from the <see cref="Command"/> class before which
        /// the command marked with the <see cref="CommandAttribute"/> must be executed.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Collections are not valid attribute parameter types")]
        public Type[] ExecuteBefore { get; set; }
    }
}
