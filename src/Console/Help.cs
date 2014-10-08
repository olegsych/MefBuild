using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using MefBuild.Properties;

namespace MefBuild
{
    [Export]
    internal class Help : Command
    {
        private readonly IList<ExportFactory<Command, CommandMetadata>> commands;

        [ImportingConstructor]
        public Help([ImportMany] IEnumerable<ExportFactory<Command, CommandMetadata>> commands)
        {
            this.commands = commands.ToList();
        }

        public override void Execute()
        {
            Console.WriteLine(Resources.UsageHeader);
            if (this.commands.Count > 0)
            {
                Console.WriteLine(Resources.AvailableCommandsHeader);
                int maxNameLength = this.commands.Max(e => e.Metadata.CommandType.Name.Length);
                foreach (ExportFactory<Command, CommandMetadata> command in this.commands)
                {
                    string commandName = command.Metadata.CommandType.Name.PadRight(maxNameLength);
                    string commandSummary = command.Metadata.Summary;
                    Console.WriteLine(Resources.AvailableCommandsLine, commandName, commandSummary);
                }
            }
        }
    }
}