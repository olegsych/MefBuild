using System;
using System.Composition;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Specifies that a command will be automatically executed after a command with the given type.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExecuteAfterAttribute : ExportAttribute
    {
        internal new const string ContractName = "ExecuteAfter";
        private readonly Type targetCommandType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteAfterAttribute"/> class with the given <see cref="Type"/> of target command.
        /// </summary>
        public ExecuteAfterAttribute(Type targetCommandType) : base(ContractName, typeof(ICommand))
        {
            const string ParameterName = "targetCommandType";

            if (targetCommandType == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (!typeof(ICommand).GetTypeInfo().IsAssignableFrom(targetCommandType.GetTypeInfo()))
            {
                throw new ArgumentException("Type derived from the MefBuild.ICommand class is expected.", ParameterName);
            }

            this.targetCommandType = targetCommandType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of target <see cref="ICommand"/>.
        /// </summary>
        public Type TargetCommandType
        {
            get { return this.targetCommandType; }
        }
    }
}
