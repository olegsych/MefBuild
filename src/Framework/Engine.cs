using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using MefBuild.Diagnostics;
using MefBuild.Execution;

namespace MefBuild
{
    /// <summary>
    /// Encapsulates <see cref="Command"/> execution logic of the MEF Build framework.
    /// </summary>
    public class Engine
    {
        private readonly CompositionContext context;
        private readonly Log log;

        /// <summary>
        /// Initializes a new instance of the <see cref="Engine"/> class with the given <see cref="CompositionContext"/>.
        /// </summary>
        public Engine(ContainerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.context = configuration
                .WithDefaultConventions(new CommandExportConventions())
                .CreateContainer();

            IEnumerable<Output> outputs = this.context.GetExports<Output>();
            this.log = new Log(outputs);
        }

        /// <summary>
        /// Executes a <see cref="Command"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// A type derived from the <see cref="Command"/> class and marked with the <see cref="ExportAttribute"/>.
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method is a strongly typed equivalent of Execute(Type).")]
        public void Execute<T>() where T : Command
        {
            this.Execute(typeof(T));
        }

        /// <summary>
        /// Executes a <see cref="Command"/> of specified <see cref="Type"/>.
        /// </summary>
        /// <param name="commandType">
        /// A <see cref="Type"/> derived from the <see cref="Command"/> class and marked with the <see cref="ExportAttribute"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="commandType"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="commandType"/> does not derive from the <see cref="Command"/> class.
        /// </exception>
        /// <exception cref="CompositionFailedException">
        /// The <paramref name="commandType"/> is not exported in the composition context of the engine. 
        /// </exception>
        public void Execute(Type commandType)
        {
            const string ParameterName = "commandType";

            if (commandType == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (!typeof(Command).GetTypeInfo().IsAssignableFrom(commandType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Type {0} does not derive from the {1}.", commandType.FullName, typeof(Command).FullName), 
                    ParameterName);
            }

            var plan = new ExecutionPlan(this.context, commandType);
            this.Execute(plan.Steps);
        }

        /// <summary>
        /// Executes a <see cref="Command"/> exported with the specified contract name.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="String"/> matching the contract name specified in the <see cref="ExportAttribute"/> applied to the command class.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="commandName"/> is null.
        /// </exception>
        /// <exception cref="CompositionFailedException">
        /// The composition context of the engine does not have a command exported with the specified contract name.
        /// </exception>
        public void Execute(string commandName)
        {
            if (commandName == null)
            {
                throw new ArgumentNullException("commandName");
            }

            var plan = new ExecutionPlan(this.context, commandName);
            this.Execute(plan.Steps);
        }

        private void Execute(IEnumerable<ExecutionStep> steps)
        {
            foreach (ExecutionStep step in steps)
            {
                this.log.CommandStarted(step);

                Command command = step.Command.Value;
                command.Log = this.log;
                command.Execute();

                this.log.CommandStopped(command);
            }
        }
    }
}