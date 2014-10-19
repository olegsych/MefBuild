using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Specifies that a command will be automatically executed after a command with the given type.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExecuteAfterAttribute : ExportAttribute
    {
        internal const string PredefinedContractName = "ExecuteAfter";
        private readonly Type targetCommandType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteAfterAttribute"/> class with the given <see cref="Type"/> of target command.
        /// </summary>
        public ExecuteAfterAttribute(Type targetCommandType) : base(PredefinedContractName, typeof(Command))
        {
            this.targetCommandType = targetCommandType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of target <see cref="Command"/>.
        /// </summary>
        public Type TargetCommandType
        {
            get { return this.targetCommandType; }
        }
    }
}
