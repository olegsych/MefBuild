using System;
using System.Collections.Generic;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Specifies <see cref="Command"/> types that will be executed before the command to which this
    /// attribute is applied.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DependsOnAttribute : Attribute
    {
        private readonly IEnumerable<Type> dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
        /// </summary>
        /// <param name="dependencies">
        /// An array of types derived from the <see cref="Command"/> class and marked with the 
        /// <see cref="ExportAttribute"/> that will be executed before the command to which this
        /// attribute is applied.
        /// </param>
        public DependsOnAttribute(params Type[] dependencies)
        {
            this.dependencies = dependencies;
        }

        /// <summary>
        /// Gets a collection of <see cref="Command"/> types that will be executed before the 
        /// command marked this attribute.
        /// </summary>
        public IEnumerable<Type> Dependencies 
        {
            get { return this.dependencies; }
        }
    }
}
