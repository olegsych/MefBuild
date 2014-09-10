using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Composition.Hosting.Core;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Encapsulates <see cref="ICommand"/> execution logic of the MEF Build framework.
    /// </summary>
    public class Engine
    {
        private static readonly MethodInfo ExecuteCommandTypeMethod = typeof(Engine)
            .GetRuntimeMethods().Single(m => m.Name == "ExecuteCommandType" && m.IsGenericMethodDefinition);

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
        /// Executes the <see cref="ICommand"/> of specified <see cref="Type"/>.
        /// </summary>
        /// <param name="commandType">A <see cref="Type"/> derived from the <see cref="ICommand"/> class.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="commandType"/> does not derive from the <see cref="ICommand"/> class.</exception>
        public void Execute(Type commandType)
        {
            const string ParameterName = "commandType";

            if (commandType == null)
            {
                throw new ArgumentNullException(ParameterName);
            }

            if (!typeof(ICommand).GetTypeInfo().IsAssignableFrom(commandType.GetTypeInfo()))
            {
                throw new ArgumentException("The type must derive from the Command class.", ParameterName);
            }

            var alreadyExecuted = new HashSet<ICommand>();
            this.ExecuteCommandType(commandType, alreadyExecuted);
        }

        private void ExecuteCommandType(Type commandType, ICollection<ICommand> alreadyExecuted)
        {
            MethodInfo genericExecuteCommandType = ExecuteCommandTypeMethod.MakeGenericMethod(commandType);
            try
            {
                genericExecuteCommandType.Invoke(this, new object[] { alreadyExecuted });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        private void ExecuteCommandType<T>(ICollection<ICommand> alreadyExecuted) where T : ICommand
        {
            var generic = this.context.GetExport<Lazy<T, Metadata>>();
            var command = new Lazy<ICommand, Metadata>(() => generic.Value, generic.Metadata);
            this.ExecuteCommand(command, alreadyExecuted);
        }

        private void ExecuteCommand(Lazy<ICommand, Metadata> command, ICollection<ICommand> alreadyExecuted)
        {
            if (alreadyExecuted.Contains(command.Value))
            {
                return;
            }

            alreadyExecuted.Add(command.Value);

            if (command.Metadata != null && command.Metadata.DependencyCommandTypes != null)
            {
                foreach (Type dependency in command.Metadata.DependencyCommandTypes)
                {
                    this.ExecuteCommandType(dependency, alreadyExecuted);
                }
            }

            this.ExecuteCommands(this.GetBeforeCommands(command.Value), alreadyExecuted);

            command.Value.Execute();

            this.ExecuteCommands(this.GetAfterCommands(command.Value), alreadyExecuted);
        }

        private void ExecuteCommands(IEnumerable<Lazy<ICommand, Metadata>> commands, ICollection<ICommand> alreadyExecuted)
        {
            foreach (Lazy<ICommand, Metadata> command in commands)
            {
                this.ExecuteCommand(command, alreadyExecuted);
            }
        }

        private IEnumerable<Lazy<ICommand, Metadata>> GetBeforeCommands(ICommand command)
        {
            return this.GetCommands(ExecuteBeforeAttribute.ContractName, command);
        }

        private IEnumerable<Lazy<ICommand, Metadata>> GetAfterCommands(ICommand command)
        {
            return this.GetCommands(ExecuteAfterAttribute.ContractName, command);
        }

        private IEnumerable<Lazy<ICommand, Metadata>> GetCommands(string contractName, ICommand targetCommand)
        {
            Type contractType = typeof(Lazy<ICommand, Metadata>[]);
            var constraints = new Dictionary<string, object> 
            { 
                { "IsImportMany", true },
                { "TargetCommandType", targetCommand.GetType() },
            };
            var contract = new CompositionContract(contractType, contractName, constraints);

            object export;
            if (this.context.TryGetExport(contract, out export))
            {
                return (IEnumerable<Lazy<ICommand, Metadata>>)export;
            }

            return Enumerable.Empty<Lazy<ICommand, Metadata>>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This class is instantiated by MEF.")]
        private class Metadata
        {
            [DefaultValue(null)]
            public IEnumerable<Type> DependencyCommandTypes { get; set; }
        }
    }
}