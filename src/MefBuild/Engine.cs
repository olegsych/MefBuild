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
        private static readonly MethodInfo ExecuteCommandDefinition = typeof(Engine)
            .GetRuntimeMethods().Single(m => m.Name == "ExecuteCommand" && m.IsGenericMethodDefinition);

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
        /// Executes an <see cref="ICommand"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="ICommand"/> interface.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This method is a stronly typed equivalent of Execute(Type).")]
        public void Execute<T>() where T : ICommand
        {
            this.ExecuteCommand<T>(new HashSet<ICommand>());
        }

        /// <summary>
        /// Executes an <see cref="ICommand"/> of specified <see cref="Type"/>.
        /// </summary>
        /// <param name="commandType">A <see cref="Type"/> that implements the <see cref="ICommand"/> interface.</param>
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

            this.ExecuteCommand(commandType, new HashSet<ICommand>());
        }

        private void ExecuteCommand(Type commandType, ICollection<ICommand> alreadyExecuted)
        {
            MethodInfo executeCommandType = ExecuteCommandDefinition.MakeGenericMethod(commandType);
            try
            {
                executeCommandType.Invoke(this, new object[] { alreadyExecuted });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        private void ExecuteCommand<T>(ICollection<ICommand> alreadyExecuted) where T : ICommand
        {
            var genericExport = this.context.GetExport<Lazy<T, Metadata>>();
            var abstractExport = new Lazy<ICommand, Metadata>(() => genericExport.Value, genericExport.Metadata);
            this.ExecuteCommand(abstractExport, alreadyExecuted);
        }

        private void ExecuteCommand(Lazy<ICommand, Metadata> commandExport, ICollection<ICommand> alreadyExecuted)
        {
            ICommand command = commandExport.Value;
            if (!alreadyExecuted.Contains(command))
            {
                alreadyExecuted.Add(command);
                this.ExecuteCommands(this.GetDependsOnCommands(commandExport), alreadyExecuted);
                this.ExecuteCommands(this.GetBeforeCommands(command.GetType()), alreadyExecuted);
                command.Execute();
                this.ExecuteCommands(this.GetAfterCommands(command.GetType()), alreadyExecuted);
            }
        }

        private void ExecuteCommands(IEnumerable<Type> commandTypes, ICollection<ICommand> alreadyExecuted)
        {
            foreach (Type commandType in commandTypes)
            {
                this.ExecuteCommand(commandType, alreadyExecuted);
            }
        }

        private void ExecuteCommands(IEnumerable<Lazy<ICommand, Metadata>> commandExports, ICollection<ICommand> alreadyExecuted)
        {
            foreach (Lazy<ICommand, Metadata> commandExport in commandExports)
            {
                this.ExecuteCommand(commandExport, alreadyExecuted);
            }
        }

        private IEnumerable<Type> GetDependsOnCommands(Lazy<ICommand, Metadata> commandExport)
        {
            return (commandExport.Metadata != null && commandExport.Metadata.DependencyCommandTypes != null)
                ? commandExport.Metadata.DependencyCommandTypes
                : Enumerable.Empty<Type>();
        }

        private IEnumerable<Lazy<ICommand, Metadata>> GetBeforeCommands(Type commandType)
        {
            return this.GetCommandExports(commandType, ExecuteBeforeAttribute.ContractName);
        }

        private IEnumerable<Lazy<ICommand, Metadata>> GetAfterCommands(Type commandType)
        {
            return this.GetCommandExports(commandType, ExecuteAfterAttribute.ContractName);
        }

        private IEnumerable<Lazy<ICommand, Metadata>> GetCommandExports(Type targetCommandType, string contractName)
        {
            Type contractType = typeof(Lazy<ICommand, Metadata>[]);
            var constraints = new Dictionary<string, object> 
            { 
                { "IsImportMany", true },
                { "TargetCommandType", targetCommandType },
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