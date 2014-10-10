using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using MefBuild.Properties;

namespace MefBuild
{
    internal class Help : Command
    {
        private Type commandType;

        public Help(Type commandType)
        {
            this.commandType = commandType;
        }

        public override void Execute()
        {
            var parameterExtractor = new ParameterMetadataExtractor();

            CompositionContext container = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithPart(this.commandType)
                .WithProvider(parameterExtractor)
                .CreateContainer();

            var command = container.GetExports<ExportFactory<Command, CommandMetadata>>()
                .Where(e => e.Metadata.CommandType == this.commandType)
                .First();

            PrintCommandSummary(command.Metadata);
            PrintCommandUsage(command.Metadata);
            PrintCommandParameters(parameterExtractor.Parameters);
        }

        private static void PrintCommandSummary(CommandMetadata command)
        {
            if (!string.IsNullOrWhiteSpace(command.Summary))
            {
                Console.WriteLine(command.Summary);
                Console.WriteLine();
            }
        }

        private static void PrintCommandUsage(CommandMetadata command)
        {
            Console.WriteLine(Resources.CommandUsageHeaderFormat, command.CommandType.Name);
        }

        private static void PrintCommandParameters(IReadOnlyCollection<ParameterMetadata> parameters)
        {
            if (parameters.Count > 0)
            {
                Console.WriteLine(Resources.ParametersHeader);

                int maxNameLength = parameters.Max(p => p.Name.Length);
                foreach (ParameterMetadata parameter in parameters)               
                {
                    Console.WriteLine(Resources.NameAndSummaryFormat, parameter.Name.PadRight(maxNameLength), parameter.Summary);
                }
            }
        }

        private class ParameterMetadata
        {
            public string Name { get; set; }

            public string Summary { get; set; }
        }

        private class ParameterMetadataExtractor : ExportDescriptorProvider
        {
            private List<ParameterMetadata> parameters = new List<ParameterMetadata>();

            public IReadOnlyCollection<ParameterMetadata> Parameters
            {
                get { return this.parameters; }
            }

            public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
            {
                string name;
                if (contract != null && contract.TryUnwrapMetadataConstraint("Name", out name, out contract))
                {
                    var parameter = new ParameterMetadata();
                    parameter.Name = name;
                    string summary;
                    if (contract.TryUnwrapMetadataConstraint("Summary", out summary, out contract))
                    {
                        parameter.Summary = summary;
                    }

                    this.parameters.Add(parameter);
                }

                return Enumerable.Empty<ExportDescriptorPromise>();
            }
        }
    }
}
