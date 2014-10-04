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
            if (this.commands.Count > 0)
            {
                Console.WriteLine(Resources.AvailableCommandsHeader);
                int maxNameLength = this.commands.Max(e => e.CreateExport().Value.GetType().Name.Length);
                foreach (ExportFactory<Command, CommandMetadata> command in this.commands)
                {
                    string commandName = command.CreateExport().Value.GetType().Name.PadRight(maxNameLength);
                    string commandSummary = command.Metadata.Summary;
                    Console.WriteLine(Resources.AvailableCommandsLine, commandName, commandSummary);
                }
            }
        }
    }
}