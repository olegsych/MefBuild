using System;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
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

            Assert.Matches(new Regex("^\\s+" + typeof(TestCommand).Name, RegexOptions.Multiline), output);
        }

        [Fact]
        public void ExecuteAlignsSummariesInAvailableCommandsSectionForReadability()
        {
            var configuration = new ContainerConfiguration().WithParts(typeof(TestCommandWithSummary), typeof(ShortWithSummary));

            string output = ExecuteHelpCommand(configuration);

            Assert.Contains(typeof(TestCommandWithSummary).Name + " " + typeof(TestCommandWithSummary).GetCustomAttribute<SummaryAttribute>().Summary, output);
            Assert.Contains(typeof(ShortWithSummary).Name + "       " + typeof(ShortWithSummary).GetCustomAttribute<SummaryAttribute>().Summary, output);
        }

        [Fact]
        public void ExecuteWritesSummaryDescriptionsOfCommandsAfterTheirNames()
        {
            var configuration = new ContainerConfiguration().WithPart<TestCommandWithSummary>();

            string output = ExecuteHelpCommand(configuration);

            string name = typeof(TestCommandWithSummary).Name;
            string summary = typeof(TestCommandWithSummary).GetCustomAttribute<SummaryAttribute>().Summary;
            Assert.Matches(name + "\\s*" + summary, output);
        }

        private static string ExecuteHelpCommand(ContainerConfiguration configuration)
        {
            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                CompositionContext context = configuration.WithPart<Help>().CreateContainer();
                var help = context.GetExport<Help>();
                help.Execute();
                return output.ToString();
            }
        }

        [Export(typeof(Command))]
        public class TestCommand : Command
        {
        }

        [Export(typeof(Command)), Summary("Test command summary")]
        public class TestCommandWithSummary : Command
        {
        }

        [Export(typeof(Command)), Summary("Short command summary")]
        public class ShortWithSummary : Command
        {
        }
    }
}
