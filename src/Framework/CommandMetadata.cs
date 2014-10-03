using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MefBuild
{
    /// <summary>
    /// Represents metadata information describing a <see cref="Command"/>.
    /// </summary>
    public class CommandMetadata
    {
        /// <summary>
        /// Gets or sets a collection of command types a <see cref="Command"/> depends on.
        /// </summary>
        /// <remarks>
        /// This collection is provided by the <see cref="DependsOnAttribute"/> applied to the command class.
        /// </remarks>
        [DefaultValue(null)]
        public IEnumerable<Type> DependsOn { get; set; }

        /// <summary>
        /// Gets or sets a summary description of a <see cref="Command"/>.
        /// </summary>
        /// <remarks>
        /// This value is provided by the <see cref="SummaryAttribute"/> applied to the command class.
        /// </remarks>
        [DefaultValue(null)]
        public string Summary { get; set; }
    }
}
