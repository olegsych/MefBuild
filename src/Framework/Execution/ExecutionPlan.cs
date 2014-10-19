﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting.Core;
using System.Linq;

namespace MefBuild.Execution
{
    internal class ExecutionPlan
    {
        private readonly List<ExecutionStep> steps;
        private readonly CompositionContext context;

        public ExecutionPlan(Type commandType, CompositionContext context)
        {
            this.steps = new List<ExecutionStep>();
            this.context = context;
            this.CreateSteps(this.GetCommand(commandType), DependencyType.None, null);
        }

        public IEnumerable<ExecutionStep> Steps
        {
            get { return this.steps; }
        }

        private bool ContainsStep(Lazy<Command, CommandMetadata> command)
        {
            return this.steps.Any(step => step.Command.Metadata.CommandType == command.Metadata.CommandType);
        }

        private void CreateSteps(
            Lazy<Command, CommandMetadata> command, 
            DependencyType dependencyType, 
            Lazy<Command, CommandMetadata> dependency)
        {
            if (!this.ContainsStep(command))
            {
                this.CreateSteps(this.GetDependsOnCommands(command), DependencyType.DependsOn, command);
                this.CreateSteps(this.GetBeforeCommands(command), DependencyType.ExecuteBefore, command);

                this.steps.Add(new ExecutionStep(command, dependencyType, dependency));

                this.CreateSteps(this.GetAfterCommands(command), DependencyType.ExecuteAfter, command);
            }
        }

        private void CreateSteps(
            IEnumerable<Lazy<Command, CommandMetadata>> commands,
            DependencyType dependencyType,
            Lazy<Command, CommandMetadata> dependency)
        {
            foreach (Lazy<Command, CommandMetadata> command in commands)
            {
                this.CreateSteps(command, dependencyType, dependency);
            }
        }

        private Lazy<Command, CommandMetadata> GetCommand(Type commandType)
        {
            var importerType = typeof(CommandExporter<>).MakeGenericType(commandType);
            var importer = (ICommandExporter)Activator.CreateInstance(importerType);
            return importer.GetCommandExport(this.context);
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetDependsOnCommands(Lazy<Command, CommandMetadata> command)
        {
            return command.Metadata.Dependencies.Select(type => this.GetCommand(type));
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetBeforeCommands(Lazy<Command, CommandMetadata> command)
        {
            return this.GetCommandExports(command.Metadata.CommandType, ExecuteBeforeAttribute.PredefinedContractName);
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetAfterCommands(Lazy<Command, CommandMetadata> command)
        {
            return this.GetCommandExports(command.Metadata.CommandType, ExecuteAfterAttribute.PredefinedContractName);
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetCommandExports(Type targetCommandType, string contractName)
        {
            Type contractType = typeof(Lazy<Command, CommandMetadata>[]);
            var constraints = new Dictionary<string, object> 
            { 
                { "IsImportMany", true },
                { "TargetCommandType", targetCommandType },
            };
            var contract = new CompositionContract(contractType, contractName, constraints);

            object export;
            if (this.context.TryGetExport(contract, out export))
            {
                return (IEnumerable<Lazy<Command, CommandMetadata>>)export;
            }

            return Enumerable.Empty<Lazy<Command, CommandMetadata>>();
        }

        private interface ICommandExporter
        {
            Lazy<Command, CommandMetadata> GetCommandExport(CompositionContext context);
        }

        private class CommandExporter<T> : ICommandExporter where T : Command
        {
            public Lazy<Command, CommandMetadata> GetCommandExport(CompositionContext context)
            {
                var export = context.GetExport<Lazy<T, CommandMetadata>>();
                export.Metadata.CommandType = typeof(T);
                return new Lazy<Command, CommandMetadata>(() => export.Value, export.Metadata);
            }
        }
    }
}
