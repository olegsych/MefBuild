using System;

namespace MefBuild.Execution
{
    /// <summary>
    /// Defines a single step in the command execution plan.
    /// </summary>
    internal class ExecutionStep
    {
        private readonly Lazy<Command, CommandMetadata> command;
        private readonly DependencyType dependencyType;
        private readonly Lazy<Command, CommandMetadata> dependency;

        public ExecutionStep(
            Lazy<Command, CommandMetadata> command, 
            DependencyType dependencyType, 
            Lazy<Command, CommandMetadata> dependency)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (dependencyType != DependencyType.None && dependency == null)
            {
                throw new ArgumentNullException("dependency");
            }

            this.command = command;
            this.dependencyType = dependencyType;
            this.dependency = dependency;
        }

        public Lazy<Command, CommandMetadata> Command
        {
            get { return this.command; }
        }

        public DependencyType DependencyType
        {
            get { return this.dependencyType; }
        }

        public Lazy<Command, CommandMetadata> Dependency
        {
            get { return this.dependency; }
        }
    }
}
