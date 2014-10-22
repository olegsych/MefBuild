using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using MefBuild.Properties;

namespace MefBuild
{
    [Export]
    internal class PrintNamedCommands : Command
    {
        private readonly IEnumerable<Type> commandTypes;

        [ImportingConstructor]
        public PrintNamedCommands([Import] IEnumerable<Type> commandTypes)
        {
            this.commandTypes = commandTypes;
        }

        public override void Execute()
        {
            IReadOnlyCollection<CommandMetadata> availableCommands = this.GetAvailableCommands();
            if (availableCommands.Count > 0)
            {
                Console.WriteLine(Resources.CommonCommandsHeader);
                PrintCommandsWithSummaries(availableCommands);
            }
        }

        private IReadOnlyCollection<CommandMetadata> GetAvailableCommands()
        {
            return this.commandTypes
                .Select(type => CreateCommandMetadata(type))
                .Where(command => !string.IsNullOrEmpty(command.Name))
                .ToList();
        }

        private static CommandMetadata CreateCommandMetadata(Type commandType)
        {
            var metadata = new CommandMetadata();

            foreach (ExportAttribute export in commandType.GetCustomAttributes<ExportAttribute>())
            {
                if (export.ContractType == typeof(Command))
                {
                    metadata.Name = export.ContractName;
                }
            }

            var summary = commandType.GetCustomAttribute<SummaryAttribute>();
            if (summary != null)
            {
                metadata.Summary = summary.Summary;
            }

            return metadata;
        }

        private static void PrintCommandsWithSummaries(IEnumerable<CommandMetadata> commands)
        {
            int maxNameLength = commands.Max(c => c.Name.Length);
            foreach (CommandMetadata command in commands)
            {
                string commandName = command.Name.PadRight(maxNameLength);
                string commandSummary = command.Summary;
                Console.WriteLine(Resources.NameAndSummaryFormat, commandName, commandSummary);
            }
        }
    }
}