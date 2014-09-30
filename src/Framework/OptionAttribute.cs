using System;
using System.Composition;

namespace MefBuild
{
    /// <summary>
    /// Provides additional metadata about an import satisfied from a command-line option.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class OptionAttribute : Attribute
    {
        private string description = string.Empty;

        /// <summary>
        /// Gets or sets the description of the command-line option.
        /// </summary>
        public string Description 
        {
            get 
            { 
                return this.description; 
            }

            set 
            { 
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.description = value; 
            }
        }
    }
}
