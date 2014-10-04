using System;
using System.Composition;
using MefBuild.Diagnostics;

namespace MefBuild
{
    /// <summary>
    /// Represents a composable command.
    /// </summary>
    public abstract class Command
    {
        private Log log;

        /// <summary>
        /// Gets or sets the <see cref="Log"/> object this instance can use to log diagnostics information.
        /// </summary>
        [Import(AllowDefault = true)]
        public Log Log 
        {
            get 
            { 
                if (this.log == null)
                {
                    this.log = Log.Empty;
                }

                return this.log; 
            }

            set 
            { 
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.log = value; 
            }
        }

        /// <summary>
        /// When overridden in a derived class, executes the command logic.
        /// </summary>
        /// <remarks>
        /// Override this method when implementing a concrete command. Commands that define structure of the build 
        /// process and don't have custom logic of their own don't have to override this method.
        /// </remarks>
        public virtual void Execute()
        {
        }
    }
}
