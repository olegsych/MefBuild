using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Encapsulates <see cref="Command"/> execution logic of the MEF Build framework.
    /// </summary>
    public class Engine
    {
        private readonly CompositionContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Engine"/> class with the given <see cref="CompositionContext"/>.
        /// </summary>
        public Engine(CompositionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
        }

        /// <summary>
        /// Executes the <see cref="Command"/> of specified <see cref="Type"/>.
        /// </summary>
        /// <param name="commandType">A <see cref="Type"/> derived from the <see cref="Command"/> class.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="commandType"/> does not derive from the <see cref="Command"/> class.</exception>
        public void Execute(Type commandType)
        {
            const string ParameterName = "commandType";

            if (commandType == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (!typeof(Command).GetTypeInfo().IsAssignableFrom(commandType.GetTypeInfo()))
            {
                throw new ArgumentException("The type must derive from the Command class.", ParameterName);
            }

            var command = (Command)this.context.GetExport(commandType);
            var executed = new HashSet<Command>();
            Execute(command, executed);
        }

        private static void Execute(Command command, ICollection<Command> executed)
        {
            if (!executed.Contains(command))
            {
                foreach (Command dependency in command.DependsOn)
                {
                    Execute(dependency, executed);
                }

                command.Execute();
                executed.Add(command);
            }
        }
    }
}