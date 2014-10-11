using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
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
        /// <typeparam name="T">A type that implements the <see cref="Command"/> interface.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method is a strongly typed equivalent of Execute(Type).")]
        public void Execute<T>() where T : Command
        {
            this.Execute(typeof(T));
        }

        /// <summary>
        /// Executes an <see cref="Command"/> of specified <see cref="Type"/>.
        /// </summary>
        /// <param name="commandType">A <see cref="Type"/> that implements the <see cref="Command"/> interface.</param>
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

            var plan = new List<ExecutionStep>();
            this.PlanCommand(commandType, plan);
            this.Execute(plan);
        }

        private void Execute(IEnumerable<ExecutionStep> plan)
        {
            foreach (ExecutionStep step in plan)
            {
                Command command = step.Command.Value;
                this.log.CommandStarted(command);
                command.Log = this.log;
                command.Execute();
                this.log.CommandStopped(command);
            }
        }

        private void PlanCommand(Type commandType, ICollection<ExecutionStep> plan)
        { 
            IEnumerable<Lazy<Command, CommandMetadata>> commandExports = this.context.GetExports<Lazy<Command, CommandMetadata>>();
            Lazy<Command, CommandMetadata> commandExport = commandExports.SingleOrDefault(c => c.Metadata.CommandType == commandType);
            if (commandExport == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Command type {0} is not exported", commandType));
            }

            this.PlanCommand(commandExport, plan);
        }

        private void PlanCommand(Lazy<Command, CommandMetadata> commandExport, ICollection<ExecutionStep> plan)
        {
            if (!plan.Any(step => step.Command.Metadata.CommandType == commandExport.Metadata.CommandType))
            {
                this.PlanCommands(GetDependsOnCommands(commandExport), plan);
                this.PlanCommands(this.GetBeforeCommands(commandExport.Metadata.CommandType), plan);

                plan.Add(new ExecutionStep(commandExport, DependencyType.None, null));

                this.PlanCommands(this.GetAfterCommands(commandExport.Metadata.CommandType), plan);
            }
        }

        private void PlanCommands(IEnumerable<Type> commandTypes, ICollection<ExecutionStep> plan)
        {
            foreach (Type commandType in commandTypes)
            {
                this.PlanCommand(commandType, plan);
            }
        }

        private void PlanCommands(IEnumerable<Lazy<Command, CommandMetadata>> commandExports, ICollection<ExecutionStep> plan)
        {
            foreach (Lazy<Command, CommandMetadata> commandExport in commandExports)
            {
                this.PlanCommand(commandExport, plan);
            }
        }

        private static IEnumerable<Type> GetDependsOnCommands(Lazy<Command, CommandMetadata> commandExport)
        {
            return (commandExport.Metadata != null && commandExport.Metadata.DependsOn != null)
                ? commandExport.Metadata.DependsOn
                : Enumerable.Empty<Type>();
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetBeforeCommands(Type commandType)
        {
            IEnumerable<Lazy<Command, CommandMetadata>> commandExports = this.context.GetExports<Lazy<Command, CommandMetadata>>();
            foreach (Lazy<Command, CommandMetadata> commandExport in commandExports)
            {
                if (commandExport.Metadata != null && 
                    commandExport.Metadata.ExecuteBefore != null &&
                    commandExport.Metadata.ExecuteBefore.Contains(commandType))
                {
                    yield return commandExport;
                }
            }
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetAfterCommands(Type commandType)
        {
            IEnumerable<Lazy<Command, CommandMetadata>> commandExports = this.context.GetExports<Lazy<Command, CommandMetadata>>();
            foreach (Lazy<Command, CommandMetadata> commandExport in commandExports)
            {
                if (commandExport.Metadata != null &&
                    commandExport.Metadata.ExecuteAfter != null &&
                    commandExport.Metadata.ExecuteAfter.Contains(commandType))
                {
                    yield return commandExport;
                }
            }
        }
    }
}