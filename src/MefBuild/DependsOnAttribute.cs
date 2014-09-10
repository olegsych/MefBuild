using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Specifies <see cref="ICommand"/> types that should be executed before the command the attribute is applied to.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
    public sealed class DependsOnAttribute : Attribute
    {
        private readonly Type[] dependencyCommandTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class with the given command types.
        /// </summary>
        /// <param name="dependencyCommandTypes">
        /// An array of <see cref="Type"/> objects representing classes that derive from <see cref="ICommand"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="dependencyCommandTypes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="dependencyCommandTypes"/> array is empty -or 
        /// the <paramref name="dependencyCommandTypes"/> array contains null elements -or- 
        /// the <paramref name="dependencyCommandTypes"/> array contains <see cref="Type"/> objects of classes that don't derive from <see cref="ICommand"/>.
        /// </exception>
        public DependsOnAttribute(params Type[] dependencyCommandTypes)
        {
            const string ParameterName = "dependencyCommandTypes";
            const string ExceptionMessage = "One or more types derived from ICommand are expected.";

            if (dependencyCommandTypes == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (dependencyCommandTypes.Length == 0)
            {
                throw new ArgumentException(ExceptionMessage, ParameterName);
            }

            foreach (Type type in dependencyCommandTypes)
            {
                if (type == null || !typeof(ICommand).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    throw new ArgumentException(ExceptionMessage, ParameterName);
                }
            }

            this.dependencyCommandTypes = dependencyCommandTypes;
        }

        /// <summary>
        /// Gets a collection of <see cref="ICommand"/> types that must be executed before the command marked with the <see cref="DependsOnAttribute"/>.
        /// </summary>
        public IEnumerable<Type> DependencyCommandTypes 
        {
            get { return this.dependencyCommandTypes; }
        }
    }
}
