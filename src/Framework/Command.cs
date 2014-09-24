using System;
using System.Composition;
using MefBuild.Hosting;

namespace MefBuild
{
    /// <summary>
    /// Represents a composable command.
    /// </summary>
    public abstract class Command
    {
        private ILog log;

        /// <summary>
        /// Gets or sets the <see cref="ILog"/> object this instance can use to log diagnostics information.
        /// </summary>
        [Import(AllowDefault = true)]
        public ILog Log 
        {
            get 
            { 
                if (this.log == null)
                {
                    this.log = new DefaultLog();
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
        /// Override this method when implementing a concrete command. Commands that only define an 
        /// injection point for commands marked with the <see cref="ExecuteBeforeAttribute"/> or 
        /// <see cref="ExecuteAfterAttribute"/>, as well as commands that define execution order 
        /// for a group of commands using the <see cref="DependsOnAttribute"/> don't have to override 
        /// this method. 
        /// </remarks>
        public virtual void Execute()
        {
        }
    }
}
