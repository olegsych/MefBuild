using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Serves as a base class for attributes that specify when a <see cref="Command"/> should be executed.
    /// </summary>
    public abstract class ExecuteAttribute : ExportAttribute
    {
        private readonly Type targetCommandType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteAttribute"/> class.
        /// </summary>
        protected ExecuteAttribute(string contractNamePrefix, Type targetCommandType) 
            : base(GetContractName(contractNamePrefix, targetCommandType), typeof(Command))
        {
            this.targetCommandType = targetCommandType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the target <see cref="Command"/>.
        /// </summary>
        public Type TargetCommandType 
        {
            get { return this.targetCommandType; }
        }

        private static string GetContractName(string contractNamePrefix, Type targetCommandType)
        {
            if (contractNamePrefix == null)
            {
                throw new ArgumentNullException("contractNamePrefix");
            }

            if (targetCommandType == null)
            {
                throw new ArgumentNullException("targetCommandType");
            }

            return contractNamePrefix + targetCommandType.FullName;
        }
    }
}
