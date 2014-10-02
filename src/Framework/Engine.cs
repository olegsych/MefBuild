using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Composition.Hosting.Core;
using System.Diagnostics.CodeAnalysis;
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
        private static readonly MethodInfo ExecuteCommandDefinition = typeof(Engine)
            .GetRuntimeMethods().Single(m => m.Name == "ExecuteCommand" && m.IsGenericMethodDefinition);

        private readonly CompositionContext context;
        private Log log;

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
            this.ExecuteCommand<T>(new HashSet<Command>());
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

        private void ExecuteCommand<T>(ICollection<Command> alreadyExecuted) where T : Command
        {
            var genericExport = this.context.GetExport<Lazy<T, Metadata>>();
            var abstractExport = new Lazy<Command, Metadata>(() => genericExport.Value, genericExport.Metadata);
            this.ExecuteCommand(abstractExport, alreadyExecuted);
        }

        private void ExecuteCommand(Lazy<Command, Metadata> commandExport, ICollection<Command> alreadyExecuted)
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

        private void ExecuteCommands(IEnumerable<Lazy<Command, Metadata>> commandExports, ICollection<Command> alreadyExecuted)
        {
            foreach (Lazy<Command, Metadata> commandExport in commandExports)
            {
                this.ExecuteCommand(commandExport, alreadyExecuted);
            }
        }

        private static IEnumerable<Type> GetDependsOnCommands(Lazy<Command, Metadata> commandExport)
        {
            return (commandExport.Metadata != null && commandExport.Metadata.DependsOn != null)
                ? commandExport.Metadata.DependsOn
                : Enumerable.Empty<Type>();
        }

        private IEnumerable<Lazy<Command, Metadata>> GetBeforeCommands(Type commandType)
        {
            return this.GetCommandExports(commandType, ExecuteBeforeAttribute.PredefinedContractName);
        }

        private IEnumerable<Lazy<Command, Metadata>> GetAfterCommands(Type commandType)
        {
            return this.GetCommandExports(commandType, ExecuteAfterAttribute.PredefinedContractName);
        }

        private IEnumerable<Lazy<Command, Metadata>> GetCommandExports(Type targetCommandType, string contractName)
        {
            Type contractType = typeof(Lazy<Command, Metadata>[]);
            var constraints = new Dictionary<string, object> 
            { 
                { "IsImportMany", true },
                { "TargetCommandType", targetCommandType },
            };
            var contract = new CompositionContract(contractType, contractName, constraints);

            object export;
            if (this.context.TryGetExport(contract, out export))
            {
                return (IEnumerable<Lazy<Command, Metadata>>)export;
            }

            return Enumerable.Empty<Lazy<Command, Metadata>>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This class is instantiated by MEF.")]
        private class Metadata
        {
            [DefaultValue(null)]
            public IEnumerable<Type> DependsOn { get; set; }
        }
    }
}