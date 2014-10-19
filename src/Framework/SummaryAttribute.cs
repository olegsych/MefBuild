using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Adds summary metadata to MEF imports and exports.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class SummaryAttribute : Attribute
    {
        private readonly string summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryAttribute"/> class.
        /// </summary>
        /// <param name="summary">
        /// A <see cref="String"/> that contains a brief description of the import or export to
        /// which the <see cref="SummaryAttribute"/> is applied.
        /// </param>
        public SummaryAttribute(string summary)
        {
            this.summary = summary;
        }

        /// <summary>
        /// Gets the summary text.
        /// </summary>
        public string Summary 
        {
            get { return this.summary; }
        }
    }
}
