using System;

namespace MefBuild
{
    /// <summary>
    /// Specifies that a command will be automatically executed after a command with the given type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExecuteAfterAttribute : ExecuteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteAfterAttribute"/> class with the given <see cref="Type"/> of target command.
        /// </summary>
        public ExecuteAfterAttribute(Type targetCommandType)
            : base("After.", targetCommandType)
        {
        }
    }
}
