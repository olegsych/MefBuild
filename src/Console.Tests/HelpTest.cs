using System;
using System.Composition;
using System.Composition.Hosting;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace MefBuild
{
    public class HelpTest
    {
        [Fact]
        public void ClassInheritsFromCommandToBeExecutedByEngine()
        {
            Assert.True(typeof(Command).IsAssignableFrom(typeof(Help)));
        }

        [Fact]
        public void ExecuteOverridesMethodInheritedFromCommandToBeInvokedPolymorphically()
        {
            Assert.Same(typeof(Command).GetMethod("Execute"), typeof(Help).GetMethod("Execute").GetBaseDefinition());
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
        public void ExecuteWritesCommandsAvailableInCompositionContextToConsoleOutput()
        {
            var configuration = new ContainerConfiguration().WithPart<TestCommand>();

            string output = ExecuteHelpCommand(configuration);

            Assert.Contains("Available commands:", output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(typeof(TestCommand).Name, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteDoesNotWriteAvailableCommandsSectionIfNoCommandsAreAvailable()
        {
            var configuration = new ContainerConfiguration();

            string output = ExecuteHelpCommand(configuration);

            Assert.DoesNotContain("Available commands:", output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteIndentsCommandNamesInAvailableCommandsSectionForReadability()
        {
            var configuration = new ContainerConfiguration().WithPart<TestCommand>();

            string output = ExecuteHelpCommand(configuration);

            Assert.Matches(new Regex(@"^\s+" + typeof(TestCommand).Name, RegexOptions.Multiline), output);
        }

        [Fact]
        public void ExecuteAlignsSummariesInAvailableCommandsSectionForReadability()
        {
            var configuration = new ContainerConfiguration().WithParts(typeof(TestCommandWithSummary), typeof(ShortWithSummary));

            string output = ExecuteHelpCommand(configuration);

            Assert.Contains(GetMetadata<TestCommandWithSummary>().CommandType.Name + " " + GetMetadata<TestCommandWithSummary>().Summary, output);
            Assert.Contains(GetMetadata<ShortWithSummary>().CommandType.Name + "       " + GetMetadata<ShortWithSummary>().Summary, output);
        }

        [Fact]
        public void ExecuteWritesSummaryDescriptionsOfCommandsAfterTheirNames()
        {
            var configuration = new ContainerConfiguration().WithPart<TestCommandWithSummary>();

            string output = ExecuteHelpCommand(configuration);

            CommandMetadata metadata = GetMetadata<TestCommandWithSummary>();
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
                    .WithPart<Help>()
                    .CreateContainer();
                var help = context.GetExport<Help>();
                help.Execute();
                return output.ToString();
            }
        }

        private static CommandMetadata GetMetadata<T>()
        {
            var configuration = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithPart<T>()
                .CreateContainer();
            var export = configuration.GetExport<ExportFactory<Command, CommandMetadata>>();
            return export.Metadata;
        }

        [Command]
        public class TestCommand : Command
        {
        }

        [Command(Summary = "Test Summary")]
        public class TestCommandWithSummary : Command
        {
        }

        [Command(Summary = "Test Summary")]
        public class ShortWithSummary : Command
        {
        }
    }
}
