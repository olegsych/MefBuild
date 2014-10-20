using System.Linq;
using MefBuild.Diagnostics;

namespace MefBuild
{
    /// <summary>
    /// Represents a composable command.
    /// </summary>
    public abstract class Command
    {
        private static readonly Log emptyLog = new Log(Enumerable.Empty<Output>());

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        protected Command()
        {
            this.Log = emptyLog;
        }

        /// <summary>
        /// Gets the <see cref="Log"/> object this instance can use to log diagnostics information.
        /// </summary>
        protected internal Log Log { get; internal set; }

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
