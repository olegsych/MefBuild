using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MefBuild
{
    /// <summary>
    /// Defines metadata information describing a concrete <see cref="Command"/> class.
    /// </summary>
    internal class CommandMetadata
    {
        [DefaultValue(null)]
        public Type CommandType { get; set; }

        [DefaultValue(null)]
        public IEnumerable<Type> DependsOn { get; set; }

        [DefaultValue(null)]
        public IEnumerable<Type> ExecuteAfter { get; set; }

        [DefaultValue(null)]
        public IEnumerable<Type> ExecuteBefore { get; set; }

        [DefaultValue(null)]
        public string Summary { get; set; }
    }
}
