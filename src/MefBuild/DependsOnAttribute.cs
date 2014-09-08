using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Specifies <see cref="Command"/> types that should be executed before the command the attribute is applied to.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
    public sealed class DependsOnAttribute : Attribute
    {
        private readonly Type[] commandTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class with the given command types.
        /// </summary>
        /// <param name="commandTypes">
        /// An array of <see cref="Type"/> objects representing classes that derive from <see cref="Command"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="commandTypes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="commandTypes"/> array is empty -or 
        /// the <paramref name="commandTypes"/> array contains null elements -or- 
        /// the <paramref name="commandTypes"/> array contains <see cref="Type"/> objects of classes that don't derive from <see cref="Command"/>.
        /// </exception>
        public DependsOnAttribute(params Type[] commandTypes)
        {
            const string ParameterName = "commandTypes";
            const string ExceptionMessage = "One or more types derived from Command are expected.";

            if (commandTypes == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (commandTypes.Length == 0)
            {
                throw new ArgumentException(ExceptionMessage, ParameterName);
            }

            foreach (Type type in commandTypes)
            {
                if (type == null || !typeof(Command).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    throw new ArgumentException(ExceptionMessage, ParameterName);
                }
            }

            this.commandTypes = commandTypes;
        }

        /// <summary>
        /// Gets a collection of <see cref="Command"/> types that must be executed before the command marked with the <see cref="DependsOnAttribute"/>.
        /// </summary>
        public IEnumerable<Type> CommandTypes 
        {
            get { return this.commandTypes; }
        }
    }
}
