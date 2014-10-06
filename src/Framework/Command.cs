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
        /// Gets the <see cref="Log"/> object this instance can use to log diagnostics information.
        /// </summary>
        protected internal Log Log 
        {
            get { return this.log ?? Log.Empty; }
            internal set { this.log = value; }
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
