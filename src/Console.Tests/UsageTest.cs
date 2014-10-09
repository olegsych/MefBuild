using System;
using System.Composition;
using System.Composition.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using MefBuild.Properties;
using Xunit;

namespace MefBuild
{
    public class UsageTest
    {
        [Fact]
        public void ClassInheritsFromCommandToBeExecutedByEngine()
        {
            Assert.True(typeof(Command).IsAssignableFrom(typeof(Usage)));
        }

        [Fact]
        public void ExecuteOverridesMethodInheritedFromCommandToBeInvokedPolymorphically()
        {
            Assert.Same(typeof(Command).GetMethod("Execute"), typeof(Usage).GetMethod("Execute").GetBaseDefinition());
        }

        [Fact]
        public void ExecuteWritesUsageSectionToConsoleOutput()
        {
            var configuration = new ContainerConfiguration();

            string output = ExecuteHelpCommand(configuration);

            Assert.Contains("Usage:", output);
            Assert.Matches(new Regex(@"^\s+MefBuild <command> \[options]", RegexOptions.Multiline), output);
            Assert.Contains("For help about specific command, type:", output);
            Assert.Matches(new Regex(@"^\s+MefBuild help <command>", RegexOptions.Multiline), output);
        }

        [Fact]
        public void ExecuteWritesCommandsWithSummaryToConsoleOutput()
        {
            var configuration = new ContainerConfiguration().WithPart<CommandWithSummary>();

            string output = ExecuteHelpCommand(configuration);

            Assert.Contains(Resources.CommonCommandsHeader, output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(typeof(CommandWithSummary).Name, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteAssumesCommandsWithoutSummaryAreInternalAndDowsNotWriteThemToConsoleOutput()
        {
            var configuration = new ContainerConfiguration().WithPart<CommandWithoutSummary>();

            string output = ExecuteHelpCommand(configuration);

            Assert.DoesNotContain(Resources.CommonCommandsHeader, output);
        }

        [Fact]
        public void ExecuteDoesNotWriteCommonCommandsSectionIfThereAreNoCommands()
        {
            var configuration = new ContainerConfiguration();

            string output = ExecuteHelpCommand(configuration);

            Assert.DoesNotContain(Resources.CommonCommandsHeader, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteIndentsCommandNamesInCommonCommandsSectionForReadability()
        {
            var configuration = new ContainerConfiguration().WithPart<CommandWithSummary>();

            string output = ExecuteHelpCommand(configuration);

            Assert.Matches(new Regex(@"^\s+" + typeof(CommandWithSummary).Name, RegexOptions.Multiline), output);
        }

        [Fact]
        public void ExecuteAlignsSummariesInCommonCommandsSectionForReadability()
        {
            var configuration = new ContainerConfiguration().WithParts(typeof(CommandWithSummary), typeof(ShortWithSummary));

            string output = ExecuteHelpCommand(configuration);

            Assert.Contains(GetMetadata<CommandWithSummary>().CommandType.Name + " " + GetMetadata<CommandWithSummary>().Summary, output);
            Assert.Contains(GetMetadata<ShortWithSummary>().CommandType.Name + "   " + GetMetadata<ShortWithSummary>().Summary, output);
        }

        [Fact]
        public void ExecuteWritesSummaryDescriptionsOfCommandsAfterTheirNames()
        {
            var configuration = new ContainerConfiguration().WithPart<CommandWithSummary>();

            string output = ExecuteHelpCommand(configuration);

            CommandMetadata metadata = GetMetadata<CommandWithSummary>();
            string name = metadata.CommandType.Name;
            string summary = metadata.Summary;
            Assert.Matches(name + "\\s*" + summary, output);
        }

        private static string ExecuteHelpCommand(ContainerConfiguration configuration)
        {
            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                CompositionContext context = configuration
                    .WithDefaultConventions(new CommandExportConventions())
                    .CreateContainer();
                var help = new Usage(context.GetExports<ExportFactory<Command, CommandMetadata>>());
                help.Execute();
                return output.ToString();
            }
        }

        private static CommandMetadata GetMetadata<T>() where T : Command
        {
            var configuration = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithPart<T>()
                .CreateContainer();
            var export = configuration.GetExport<ExportFactory<Command, CommandMetadata>>();
            return export.Metadata;
        }

        [Command]
        public class CommandWithoutSummary : Command
        {
        }

        [Command(Summary = "Test Summary")]
        public class CommandWithSummary : Command
        {
        }

        [Command(Summary = "Test Summary")]
        public class ShortWithSummary : Command
        {
        }
    }
}
