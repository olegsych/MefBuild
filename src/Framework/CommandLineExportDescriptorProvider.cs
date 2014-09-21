using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Linq;

namespace MefBuild
{
    /// <summary>
    /// Satisfies composition export requests using command-line arguments.
    /// </summary>
    public class CommandLineExportDescriptorProvider : ExportDescriptorProvider
    {
        private readonly IEnumerable<CommandLineArgument> arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineExportDescriptorProvider"/> class 
        /// with the given array of command line arguments.
        /// </summary>
        public CommandLineExportDescriptorProvider(string[] commandLineArguments)
        {
            if (commandLineArguments == null)
            {
                throw new ArgumentNullException("commandLineArguments");
            }

            this.arguments = commandLineArguments.Select(str => new CommandLineArgument(str));
        }

        /// <summary>
        /// Returns a collection of <see cref="ExportDescriptorPromise"/> objects created from command-line arguments
        /// with names matching the requested contract name.
        /// </summary>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (IsSingleImport(contract))
            {
                foreach (CommandLineArgument argument in this.arguments.Where(arg => arg.Name == contract.ContractName))
                {
                    object value = Convert.ChangeType(argument.Value, contract.ContractType);
                    yield return new ExportDescriptorPromise(
                        contract,
                        string.Format("Command-line argument: '{0}'", argument.Original),
                        true,
                        NoDependencies,
                        _ => ExportDescriptor.Create((c, o) => value, NoMetadata));
                }
            }
        }

        private static bool IsSingleImport(CompositionContract contract)
        {
            bool isImportMany;
            CompositionContract importManyContract;
            if (contract.TryUnwrapMetadataConstraint("IsImportMany", out isImportMany, out importManyContract))
            {
                return !isImportMany;
            }

            return true;
        }
    }
}
