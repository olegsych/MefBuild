using System;
using System.Collections.Generic;
using System.Linq;

namespace MefBuild
{
    /// <summary>
    /// Serves as a base class for composable commands.
    /// </summary>
    public abstract class Command
    {
        private readonly IEnumerable<Command> dependsOn;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class without dependencies. 
        /// </summary>
        protected Command()
        {
            this.dependsOn = Enumerable.Empty<Command>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class with the specified dependencies.
        /// </summary>
        /// <param name="dependsOn">A read-only collection of <see cref="Command"/> objects this instance will execute.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dependsOn"/> argument is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dependsOn"/> array is empty contains null elements.</exception>
        protected Command(params Command[] dependsOn)
        {
            if (dependsOn == null)
            {
                throw new ArgumentNullException("dependsOn");
            }

            int commandCount = 0;
            foreach (Command command in dependsOn)
            {
                commandCount++;
                if (command == null)
                {
                    throw new ArgumentException("Command objects cannot be null.");
                }
            }

            if (commandCount == 0)
            {
                throw new ArgumentException("One or more Command objects expected.");
            }

            this.dependsOn = dependsOn;
        }

        /// <summary>
        /// Gets a collection of <see cref="Command"/> objects this command depends on.
        /// </summary>
        /// <remarks>
        /// This property mimics the DependsOnTargets attribute of MSBuild targets.
        /// <a href="http://msdn.microsoft.com/en-us/library/t50z2hka.aspx"/>.
        /// </remarks>
        public IEnumerable<Command> DependsOn 
        {
            get { return this.dependsOn; }
        }

        /// <summary>
        /// When overridden in a child class, executes the command.
        /// </summary>
        public virtual void Execute()
        {
        }
    }
}
