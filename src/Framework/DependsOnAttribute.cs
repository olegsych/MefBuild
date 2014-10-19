using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

namespace MefBuild
{
    /// <summary>
    /// Adds dependency metadata to concrete <see cref="Command"/> classes.
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
        /// An array of <see cref="Command"/> types that must be executed before the command
        /// marked with the <see cref="DependsOnAttribute"/>.
        /// </param>
        public DependsOnAttribute(params Type[] dependencies)
        {
            this.dependencies = dependencies;
        }

        /// <summary>
        /// Gets a collection of <see cref="Command"/> types that must be executed before the 
        /// command marked with the <see cref="DependsOnAttribute"/>.
        /// </summary>
        public IEnumerable<Type> Dependencies 
        {
            get { return this.dependencies; }
        }
    }
}
