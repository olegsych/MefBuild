using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MefBuild
{
    /// <summary>
    /// Defines metadata information describing a concrete <see cref="Command"/> class.
    /// </summary>
    internal class CommandMetadata
    {
        private IEnumerable<Type> dependencies;

        [DefaultValue(null)]
        public Type CommandType { get; set; }

        [DefaultValue(null)]
        public IEnumerable<Type> Dependencies 
        {
            get { return this.dependencies ?? Enumerable.Empty<Type>(); }
            set { this.dependencies = value; }
        }

        [DefaultValue(null)]
        public string Summary { get; set; }
    }
}
