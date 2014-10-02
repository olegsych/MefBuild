using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Provides a summary describing an exported command or an imported property or parameter.
    /// </summary>
    /// <remarks>
    /// Summary is used by the MefBuild.exe to display help information about available commands and their options.
    /// </remarks>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class SummaryAttribute : Attribute
    {
        private readonly string summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryAttribute"/> class with the given <paramref name="summary"/> text.
        /// </summary>
        public SummaryAttribute(string summary)
        {
            if (summary == null)
            {
                throw new ArgumentNullException("summary");
            }

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
