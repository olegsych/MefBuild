using System;

namespace MefBuild
{
    /// <summary>
    /// Specifies that a command will be automatically executed before a command with the given type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExecuteBeforeAttribute : ExecuteAttribute
    {
        internal const string ContractNamePrefix = "Before.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteBeforeAttribute"/> class with the given <see cref="Type"/> of target command.
        /// </summary>
        public ExecuteBeforeAttribute(Type targetCommandType) : base(ContractNamePrefix, targetCommandType)
        {
        }
    }
}
