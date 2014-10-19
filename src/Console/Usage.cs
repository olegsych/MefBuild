using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using MefBuild.Properties;

namespace MefBuild
{
    [Shared, Export(typeof(Command)), Summary("Displays help information about MefBuild and other commands.")]
    internal class Usage : Command
    {
        private readonly IList<ExportFactory<Command, CommandMetadata>> commands;

        [ImportingConstructor]
        public Usage([ImportMany] IEnumerable<ExportFactory<Command, CommandMetadata>> commands)
        {
            this.commands = commands.ToList();
        }

        public override void Execute()
        {
            PrintUsage();
            this.PrintCommonCommands();
        }

        private static void PrintUsage()
        {
            Console.WriteLine(Resources.UsageHeader);
        }

        private IReadOnlyCollection<CommandMetadata> GetCommonCommands()
        {
            return this.commands
                .Where(e => !string.IsNullOrWhiteSpace(e.Metadata.Summary))
                .Select(e => e.Metadata)
                .ToList();            
        }

        private void PrintCommonCommands()
        {
            IReadOnlyCollection<CommandMetadata> commonCommands = this.GetCommonCommands();
            if (commonCommands.Count > 0)
            {
                Console.WriteLine(Resources.CommonCommandsHeader);
                PrintCommandsWithSummaries(commonCommands);
            }
        }

        private static void PrintCommandsWithSummaries(IEnumerable<CommandMetadata> commands)
        {
            int maxNameLength = commands.Max(c => c.CommandType.Name.Length);
            foreach (CommandMetadata command in commands)
            {
                string commandName = command.CommandType.Name.PadRight(maxNameLength);
                string commandSummary = command.Summary;
                Console.WriteLine(Resources.NameAndSummaryFormat, commandName, commandSummary);
            }
        }
    }
}