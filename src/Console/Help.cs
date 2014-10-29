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
            var argumentExtractor = new ArgumentDescriptorExtractor();

            CompositionContext container = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithAssemblies(this.assemblies)
                .WithProvider(argumentExtractor)
                .CreateContainer();

            IEnumerable<Lazy<Command, CommandMetadata>> allCommands = container.GetExports<Lazy<Command, CommandMetadata>>();

            var command = allCommands.First(e => e.Metadata.CommandType == this.commandType);
            PrintCommandSummary(command.Metadata);
            PrintCommandUsage(command.Metadata);

            var plan = new ExecutionPlan(container, this.commandType);
            foreach (ExecutionStep step in plan.Steps)
            {
                Command forceArgumentImport = step.Command.Value;
            }

            PrintCommandArguments(argumentExtractor.Arguments);
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

        private static void PrintCommandArguments(IReadOnlyCollection<ArgumentDescriptor> arguments)
        {
            if (arguments.Count > 0)
            {
                Console.WriteLine(Resources.ArgumentsHeader);

                int maxNameLength = arguments.Max(p => p.Name.Length);
                foreach (ArgumentDescriptor argument in arguments)               
                {
                    Console.WriteLine(Resources.NameAndSummaryFormat, argument.Name.PadRight(maxNameLength), argument.Summary);
                }
            }
        }

        private class ArgumentDescriptor
        {
            public string Name { get; set; }

            public string Summary { get; set; }
        }

        private class ArgumentDescriptorExtractor : ExportDescriptorProvider
        {
            private List<ArgumentDescriptor> arguments = new List<ArgumentDescriptor>();

            public IReadOnlyCollection<ArgumentDescriptor> Arguments
            {
                get { return this.arguments; }
            }

            public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
            {
                if (IsArgument(contract))
                {
                    var argument = new ArgumentDescriptor();
                    argument.Name = contract.ContractName;
                    argument.Summary = GetSummary(contract);

                    Func<IEnumerable<CompositionDependency>, ExportDescriptor> exportFactory = dependencies =>
                    {
                        CompositeActivator instanceFactory = (c, o) =>
                        {
                            this.arguments.Add(argument);
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
                        string.Format(CultureInfo.CurrentCulture, "Argument: '{0}'", contract.ContractName),
                        true,
                        NoDependencies,
                        exportFactory);

                    return new[] { promise };
                }

                return Enumerable.Empty<ExportDescriptorPromise>();
            }

            private static bool IsArgument(CompositionContract contract)
            {
                bool isCommandLineArgument;
                return !string.IsNullOrEmpty(contract.ContractName) &&
                    contract.TryUnwrapMetadataConstraint("IsArgument", out isCommandLineArgument, out contract) &&
                    isCommandLineArgument;
            }

            private static string GetSummary(CompositionContract contract)
            {
                string summary;
                if (contract.TryUnwrapMetadataConstraint("Summary", out summary, out contract))
                {
                    return summary;
                }

                return string.Empty;
            }
        }
    }
}
