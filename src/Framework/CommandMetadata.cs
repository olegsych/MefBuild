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
        private IEnumerable<Type> dependsOn;
        private IEnumerable<Type> executeAfter;
        private IEnumerable<Type> executeBefore;

        [DefaultValue(null)]
        public Type CommandType { get; set; }

        [DefaultValue(null)]
        public IEnumerable<Type> DependsOn 
        {
            get { return this.dependsOn ?? Enumerable.Empty<Type>(); }
            set { this.dependsOn = value; }
        }

        [DefaultValue(null)]
        public IEnumerable<Type> ExecuteAfter 
        {
            get { return this.executeAfter ?? Enumerable.Empty<Type>(); }
            set { this.executeAfter = value; }
        }

        [DefaultValue(null)]
        public IEnumerable<Type> ExecuteBefore 
        {
            get { return this.executeBefore ?? Enumerable.Empty<Type>(); }
            set { this.executeBefore = value; }
        }

        [DefaultValue(null)]
        public string Summary { get; set; }
    }
}
