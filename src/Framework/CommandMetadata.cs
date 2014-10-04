using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MefBuild
{
    /// <summary>
    /// Represents metadata information describing a concrete <see cref="Command"/> class.
    /// </summary>
    public class CommandMetadata
    {
        /// <summary>
        /// Gets or sets the type of the concrete <see cref="Command"/> class.
        /// </summary>
        [DefaultValue(null)]
        public Type CommandType { get; set; }

        /// <summary>
        /// Gets or sets a collection of command types the <see cref="Command"/> depends on.
        /// </summary>
        [DefaultValue(null)]
        public IEnumerable<Type> DependsOn { get; set; }

        /// <summary>
        /// Gets or sets a summary description of the concrete <see cref="Command"/> class.
        /// </summary>
        [DefaultValue(null)]
        public string Summary { get; set; }
    }
}
