using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MefBuild.Diagnostics;

namespace MefBuild
{
    /// <summary>
    /// Encapsulates <see cref="Command"/> execution logic of the MEF Build framework.
    /// </summary>
    public class Engine
    {
        private readonly CompositionContext context;
        private Log log;

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
            this.context.SatisfyImports(this);            
        }

        /// <summary>
        /// Gets or sets the <see cref="Log"/> object this instance uses to log diagnostics information.
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
        /// Executes an <see cref="Command"/> of type <typeparamref name="T"/>.
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

            this.ExecuteCommand(commandType, new HashSet<Command>());
        }

        private void ExecuteCommand(Type commandType, ICollection<Command> alreadyExecuted)
        {
            IEnumerable<Lazy<Command, CommandMetadata>> commandExports = this.context.GetExports<Lazy<Command, CommandMetadata>>();
            Lazy<Command, CommandMetadata> commandExport = commandExports.SingleOrDefault(c => c.Metadata.CommandType == commandType);
            if (commandExport == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Command type {0} is not exported", commandType));
            }

            this.ExecuteCommand(commandExport, alreadyExecuted);
        }

        private void ExecuteCommand(Lazy<Command, CommandMetadata> commandExport, ICollection<Command> alreadyExecuted)
        {
            Command command = commandExport.Value;
            if (!alreadyExecuted.Contains(command))
            {
                alreadyExecuted.Add(command);

                this.ExecuteCommands(GetDependsOnCommands(commandExport), alreadyExecuted);
                this.ExecuteCommands(this.GetBeforeCommands(command.GetType()), alreadyExecuted);

                this.context.SatisfyImports(command); // from the dependency and before commands

                this.Log.CommandStarted(command);
                command.Execute();
                this.Log.CommandStopped(command);

                this.ExecuteCommands(this.GetAfterCommands(command.GetType()), alreadyExecuted);
            }
        }

        private void ExecuteCommands(IEnumerable<Type> commandTypes, ICollection<Command> alreadyExecuted)
        {
            foreach (Type commandType in commandTypes)
            {
                this.ExecuteCommand(commandType, alreadyExecuted);
            }
        }

        private void ExecuteCommands(IEnumerable<Lazy<Command, CommandMetadata>> commandExports, ICollection<Command> alreadyExecuted)
        {
            foreach (Lazy<Command, CommandMetadata> commandExport in commandExports)
            {
                this.ExecuteCommand(commandExport, alreadyExecuted);
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