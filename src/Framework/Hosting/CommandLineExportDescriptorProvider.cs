using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MefBuild.Hosting
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
                IEnumerable<CommandLineArgument> matchingArguments = this.arguments
                    .Where(arg => string.Equals(arg.Name, contract.ContractName, StringComparison.OrdinalIgnoreCase));

                foreach (CommandLineArgument argument in matchingArguments)
                {
                    object value = ConvertStringToType(argument.Value, contract.ContractType);
                    yield return new ExportDescriptorPromise(
                        contract,
                        string.Format("Command-line argument: '{0}'", argument.Original),
                        true,
                        NoDependencies,
                        _ => ExportDescriptor.Create((c, o) => value, NoMetadata));
                }
            }
        }

        private static object ConvertStringToType(string value, Type type)
        {
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, value);
            }

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        private static bool IsSingleImport(CompositionContract contract)
        {
            bool isImportMany;
            CompositionContract importManyContract;
            if (contract.TryUnwrapMetadataConstraint("IsImportMany", out isImportMany, out importManyContract))
            {
                return !isImportMany;
            }

            // Single import of collection properties is currently not supported. I.e. [Import("Argument")] IEnumerable<string> Arguments
            if (contract.ContractType.IsArray)
            {
                return false;
            }

            if (contract.ContractType.GetTypeInfo().IsGenericType)
            {
                if (typeof(IEnumerable<>).GetTypeInfo().IsAssignableFrom(contract.ContractType.GetGenericTypeDefinition().GetTypeInfo()) ||
                    typeof(IList<>).GetTypeInfo().IsAssignableFrom(contract.ContractType.GetGenericTypeDefinition().GetTypeInfo()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
