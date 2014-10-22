using System;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MefBuild.Properties;
using Xunit;

namespace MefBuild
{
    public class PrintNamedCommandsTest
    {
        [Fact]
        public void ExecuteWritesCommandsExportedWithContractNameToConsoleOutput()
        {
            string output = PrintAvailableCommands(typeof(NamedCommand));
            string commandName = typeof(NamedCommand).GetCustomAttribute<ExportAttribute>().ContractName;
            Assert.Contains(commandName, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteDoesNotWriteCommandsExportedWithoutContractNameToConsoleOutput()
        {
            string output = PrintAvailableCommands(typeof(TypedCommand));
            Assert.DoesNotContain(typeof(TypedCommand).Name, output);
        }

        [Fact]
        public void ExecuteSupportsCommandExportedWithBothNameAndType()
        {
            string output = PrintAvailableCommands(typeof(TypedNamedCommand));
            string commandName = typeof(TypedNamedCommand).GetCustomAttributes<ExportAttribute>()
                .Single(a => !string.IsNullOrWhiteSpace(a.ContractName)).ContractName;
            Assert.Contains(commandName, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteGroupsAvailableCommandsInSeparateSectionForReadability()
        {
            string output = PrintAvailableCommands(typeof(NamedCommand));
            Assert.Contains(Resources.CommonCommandsHeader, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteDoesNotWriteCommonCommandsSectionIfThereAreNoCommands()
        {
            string output = PrintAvailableCommands();
            Assert.DoesNotContain(Resources.CommonCommandsHeader, output, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ExecuteIndentsCommandNamesInCommonCommandsSectionForReadability()
        {
            string output = PrintAvailableCommands(typeof(NamedCommand));
            string commandName = typeof(NamedCommand).GetCustomAttribute<ExportAttribute>().ContractName;
            Assert.Matches(new Regex(@"^\s+" + commandName, RegexOptions.Multiline), output);
        }

        [Fact]
        public void ExecuteAlignsSummariesInCommonCommandsSectionForReadability()
        {
            string output = PrintAvailableCommands(typeof(LoooooooongNameWithSummary), typeof(ShortNameWithSummary));

            string loooooooongName = typeof(LoooooooongNameWithSummary).GetCustomAttribute<ExportAttribute>().ContractName;
            string longSummary = typeof(LoooooooongNameWithSummary).GetCustomAttribute<SummaryAttribute>().Summary;
            string shortName = typeof(ShortNameWithSummary).GetCustomAttribute<ExportAttribute>().ContractName;
            string shortSummary = typeof(ShortNameWithSummary).GetCustomAttribute<SummaryAttribute>().Summary;
            Assert.Contains(loooooooongName + " " + longSummary, output);
            Assert.Contains(shortName + "       " + shortSummary, output);
        }

        [Fact]
        public void ExecuteWritesSummaryDescriptionsOfCommandsAfterTheirNames()
        {
            string output = PrintAvailableCommands(typeof(ShortNameWithSummary));

            string name = typeof(ShortNameWithSummary).GetCustomAttribute<ExportAttribute>().ContractName;
            string summary = typeof(ShortNameWithSummary).GetCustomAttribute<SummaryAttribute>().Summary;
            Assert.Matches(name + "\\s*" + summary, output);
        }

        private static string PrintAvailableCommands(params Type[] commandTypes)
        {
            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                var help = new PrintNamedCommands(commandTypes);
                help.Execute();
                return output.ToString();
            }
        }

        [Export("Named", typeof(Command))]
        public class NamedCommand : Command
        {
        }

        [Export]
        public class TypedCommand : Command
        {
        }

        [Export, Export("Named", typeof(Command))]
        public class TypedNamedCommand : Command
        {
        }

        [Export("Loooooooong", typeof(Command)), Summary("Test Summary")]
        public class LoooooooongNameWithSummary : Command
        {
        }

        [Export("Short", typeof(Command)), Summary("Test Summary")]
        public class ShortNameWithSummary : Command
        {
        }
    }
}
