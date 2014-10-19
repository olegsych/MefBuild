using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Globalization;
using System.Linq;
using System.Reflection;
using MefBuild.Execution;
using MefBuild.Properties;

namespace MefBuild
{
    internal class Help : Command
    {
        private readonly Type commandType;
        private readonly IEnumerable<Assembly> assemblies;

        public Help(Type commandType, IEnumerable<Assembly> assemblies)
        {
            this.commandType = commandType;
            this.assemblies = assemblies.Concat(new[] { commandType.Assembly }).Distinct();
        }

        public override void Execute()
        {
            var parameterExtractor = new ParameterMetadataExtractor();

            CompositionContext container = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithAssemblies(this.assemblies)
                .WithProvider(parameterExtractor)
                .CreateContainer();

            IEnumerable<Lazy<Command, CommandMetadata>> allCommands = container.GetExports<Lazy<Command, CommandMetadata>>();

            var command = allCommands.First(e => e.Metadata.CommandType == this.commandType);
            PrintCommandSummary(command.Metadata);
            PrintCommandUsage(command.Metadata);

            var plan = new ExecutionPlan(this.commandType, allCommands);
            foreach (ExecutionStep step in plan.Steps)
            {
                Command forceParameterImport = step.Command.Value;
            }

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
                    CompositionContract contractWithoutSummary;
                    if (contract.TryUnwrapMetadataConstraint("Summary", out summary, out contractWithoutSummary))
                    {
                        parameter.Summary = summary;
                        contract = contractWithoutSummary;
                    }

                    Func<IEnumerable<CompositionDependency>, ExportDescriptor> exportFactory = dependencies =>
                    {
                        CompositeActivator instanceFactory = (c, o) =>
                        {
                            this.parameters.Add(parameter);
                            if (contract.ContractType.IsValueType)
                            {
                                return Activator.CreateInstance(contract.ContractType);
                            }
                            else
                            {
                                return null;
                            }
                        };
    
                        return ExportDescriptor.Create(instanceFactory, NoMetadata);
                    };

                    var promise = new ExportDescriptorPromise(
                        contract,
                        string.Format(CultureInfo.CurrentCulture, "Parameter: '{0}'", name),
                        true,
                        NoDependencies,
                        exportFactory);

                    return new[] { promise };
                }

                return Enumerable.Empty<ExportDescriptorPromise>();
            }
        }
    }
}
