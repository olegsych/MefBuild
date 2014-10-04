using System;
using System.Collections.Generic;
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
        private readonly Type commandType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="commandType">A <see cref="Type"/> of the concrete <see cref="Command"/> class.</param>
        public CommandAttribute(Type commandType) : base(typeof(Command))
        {
            this.commandType = commandType;
        }

        /// <summary>
        /// Gets the type of the concrete <see cref="Command"/> class.
        /// </summary>
        public Type CommandType
        {
            get { return this.commandType; }
        }

        /// <summary>
        /// Gets or sets a collection of types derived from <see cref="Command"/> that mast be executed before the
        /// command marked with the <see cref="CommandAttribute"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Collections are not valid attribute parameter types")]
        public Type[] DependsOn { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the concrete <see cref="Command"/> class.
        /// </summary>
        public string Summary { get; set; }
    }
}
