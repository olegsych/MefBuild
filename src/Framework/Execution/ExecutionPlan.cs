using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MefBuild.Execution
{
    internal class ExecutionPlan
    {
        private readonly List<ExecutionStep> steps;
        private readonly IEnumerable<Lazy<Command, CommandMetadata>> allCommands;

        public ExecutionPlan(Type commandType, IEnumerable<Lazy<Command, CommandMetadata>> allCommands)
        {
            this.steps = new List<ExecutionStep>();
            this.allCommands = allCommands;
            this.CreateSteps(this.GetCommand(commandType));
        }

        public IEnumerable<ExecutionStep> Steps
        {
            get { return this.steps; }
        }

        private bool ContainsStep(Lazy<Command, CommandMetadata> command)
        {
            return this.steps.Any(step => step.Command.Metadata.CommandType == command.Metadata.CommandType);
        }

        private void CreateSteps(Lazy<Command, CommandMetadata> command)
        {
            if (!this.ContainsStep(command))
            {
                this.CreateSteps(this.GetDependsOnCommands(command));
                this.CreateSteps(this.GetBeforeCommands(command));

                this.steps.Add(new ExecutionStep(command, DependencyType.None, null));

                this.CreateSteps(this.GetAfterCommands(command));
            }
        }

        private void CreateSteps(IEnumerable<Lazy<Command, CommandMetadata>> commands)
        {
            foreach (Lazy<Command, CommandMetadata> command in commands)
            {
                this.CreateSteps(command);
            }
        }

        private Lazy<Command, CommandMetadata> GetCommand(Type commandType)
        {
            Lazy<Command, CommandMetadata> command = this.allCommands.SingleOrDefault(c => c.Metadata.CommandType == commandType);
            if (command == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Command type {0} is not exported", commandType));
            }

            return command;
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetDependsOnCommands(Lazy<Command, CommandMetadata> command)
        {
            return command.Metadata.DependsOn.Select(type => this.GetCommand(type));
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetBeforeCommands(Lazy<Command, CommandMetadata> command)
        {
            return this.allCommands.Where(c => c.Metadata.ExecuteBefore.Contains(command.Metadata.CommandType));
        }

        private IEnumerable<Lazy<Command, CommandMetadata>> GetAfterCommands(Lazy<Command, CommandMetadata> command)
        {
            return this.allCommands.Where(c => c.Metadata.ExecuteAfter.Contains(command.Metadata.CommandType));
        }
    }
}
