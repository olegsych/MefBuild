﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Reflection;

namespace MefBuild
{
    /// <summary>
    /// Encapsulates <see cref="Command"/> execution logic of the MEF Build framework.
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

            var alreadyExecuted = new HashSet<Command>();
            this.ExecuteCommandType(commandType, alreadyExecuted);
        }

        private void ExecuteCommandType(Type commandType, ICollection<Command> alreadyExecuted)
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

        private void ExecuteCommandType<T>(ICollection<Command> alreadyExecuted) where T : Command
        {
            var generic = this.context.GetExport<Lazy<T, Metadata>>();
            var command = new Lazy<Command, Metadata>(() => generic.Value, generic.Metadata);
            this.ExecuteCommand(command, alreadyExecuted);
        }

        private void ExecuteCommand(Lazy<Command, Metadata> command, ICollection<Command> alreadyExecuted)
        {
            if (alreadyExecuted.Contains(command.Value))
            {
                return;
            }

            alreadyExecuted.Add(command.Value);

            if (command.Metadata != null && command.Metadata.CommandTypes != null)
            {
                foreach (Type dependency in command.Metadata.CommandTypes)
                {
                    this.ExecuteCommandType(dependency, alreadyExecuted);
                }
            }

            this.ExecuteCommands(this.GetBeforeCommands(command.Value), alreadyExecuted);

            command.Value.Execute();

            this.ExecuteCommands(this.GetAfterCommands(command.Value), alreadyExecuted);
        }

        private void ExecuteCommands(IEnumerable<Lazy<Command, Metadata>> commands, ICollection<Command> alreadyExecuted)
        {
            foreach (Lazy<Command, Metadata> command in commands)
            {
                this.ExecuteCommand(command, alreadyExecuted);
            }
        }

        private IEnumerable<Lazy<Command, Metadata>> GetBeforeCommands(Command command)
        {
            return this.GetCommands(ExecuteBeforeAttribute.ContractName, command);
        }

        private IEnumerable<Lazy<Command, Metadata>> GetAfterCommands(Command command)
        {
            return this.GetCommands(ExecuteAfterAttribute.ContractName, command);
        }

        private IEnumerable<Lazy<Command, Metadata>> GetCommands(string contractName, Command targetCommand)
        {
            Type contractType = typeof(Lazy<Command, Metadata>[]);
            var constraints = new Dictionary<string, object> 
            { 
                { "IsImportMany", true },
                { "TargetCommandType", targetCommand.GetType() },
            };
            var contract = new CompositionContract(contractType, contractName, constraints);

            object export;
            if (this.context.TryGetExport(contract, out export))
            {
                return (IEnumerable<Lazy<Command, Metadata>>)export;
            }

            return Enumerable.Empty<Lazy<Command, Metadata>>();
        }
        
        private class Metadata
        {
            [DefaultValue(null)]
            public IEnumerable<Type> CommandTypes { get; set; }
        }
    }
}