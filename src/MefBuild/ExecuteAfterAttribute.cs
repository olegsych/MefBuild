using System;
using System.Composition;
using System.Globalization;
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
        public ExecuteAfterAttribute(Type targetCommandType) : base(ContractName, typeof(Command))
        {
            const string ParameterName = "targetCommandType";

            if (targetCommandType == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (!typeof(Command).GetTypeInfo().IsAssignableFrom(targetCommandType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Type derived from the {0} class is expected.", typeof(Command)),
                    ParameterName);
            }

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
