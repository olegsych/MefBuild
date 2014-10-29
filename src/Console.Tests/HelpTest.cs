using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace MefBuild
{
    public sealed class HelpTest : IDisposable
    {
        private readonly ConsoleOutputInterceptor console = new ConsoleOutputInterceptor();

        public void Dispose()
        {
            this.console.Dispose();
        }

        [Fact]
        public void ExecuteOverridesCommandMethodToBeInvokedPolymorphically()
        {
            Assert.Same(typeof(Command).GetMethod("Execute"), typeof(Help).GetMethod("Execute").GetBaseDefinition());
        }

        [Fact]
        public void ExecuteWritesCommandSummaryToConsoleOutput()
        {
            Help help = NewHelp(typeof(CommandWithSummary));

            help.Execute();

            var summaryAttribute = typeof(CommandWithSummary).GetCustomAttribute<SummaryAttribute>();
            Assert.Contains(summaryAttribute.Summary, this.console.Output);
        }

        [Fact]
        public void ExecuteDoesNotWriteBlankLineWhenCommandDoesNotHaveSummary()
        {
            Help help = NewHelp(typeof(CommandWithoutSummaryAndArguments));

            help.Execute();

            Assert.StartsWith("Usage", this.console.Output);
        }

        [Fact]
        public void ExecuteWritesCommandUsageToConsoleOutput()
        {
            Help help = NewHelp(typeof(CommandWithoutSummaryAndArguments));

            help.Execute();

            Assert.Contains("Usage:", this.console.Output);
            Assert.Matches(new Regex(@"^\s+MefBuild " + typeof(CommandWithoutSummaryAndArguments).Name, RegexOptions.Multiline), this.console.Output);
        }

        [Fact]
        public void ExecuteWritesArgumentsSectionHeader()
        {
            Help help = NewHelp(typeof(CommandWithArguments));

            help.Execute();

            Assert.Contains("Arguments:", this.console.Output);
        }

        [Fact]
        public void ExecuteDoesNotWriteArgumentsSectionHeaderIfCommandHasNoArguments()
        {
            Help help = NewHelp(typeof(CommandWithSummary));

            help.Execute();

            Assert.DoesNotContain("Arguments:", this.console.Output);
        }

        [Fact]
        public void ExecuteWritesArgumentNamesAndSummariesToConsoleOutput()
        {
            Help help = NewHelp(typeof(CommandWithArguments));

            help.Execute();

            var import = typeof(CommandWithArguments).GetProperty("Property").GetCustomAttribute<ImportAttribute>();
            var summary = typeof(CommandWithArguments).GetProperty("Property").GetCustomAttribute<SummaryAttribute>();
            Assert.Matches(new Regex(import.ContractName + @"\s+" + summary.Summary), this.console.Output);
        }
        
        [Fact]
        public void ExecuteIndentsArgumentNamesToImproveReadability()
        {
            Help help = NewHelp(typeof(CommandWithArguments));

            help.Execute();

            var import = typeof(CommandWithArguments).GetProperty("Property").GetCustomAttribute<ImportAttribute>();
            Assert.Matches(new Regex(@"^\s+" + import.ContractName, RegexOptions.Multiline), this.console.Output);
        }

        [Fact]
        public void ExecuteAlignsArgumentSummariesToImproveReadability()
        {
            Help help = NewHelp(typeof(CommandWithArguments));

            help.Execute();

            var argumentName = typeof(CommandWithArguments).GetProperty("Property").GetCustomAttribute<ImportAttribute>().ContractName;
            var argumentSummary = typeof(CommandWithArguments).GetProperty("Property").GetCustomAttribute<SummaryAttribute>().Summary;
            var longArgumentName = typeof(CommandWithArguments).GetProperty("LongProperty").GetCustomAttribute<ImportAttribute>().ContractName;
            var longArgumentSummary = typeof(CommandWithArguments).GetProperty("LongProperty").GetCustomAttribute<SummaryAttribute>().Summary;
            Assert.Contains(argumentName + "     " + argumentSummary, this.console.Output);
            Assert.Contains(longArgumentName + " " + longArgumentSummary, this.console.Output);
        }

        [Fact]
        public void ExecuteListsArgumentsOfCommandsTargetCommandsDependsOn()
        {
            Help help = NewHelp(typeof(DependentWithoutArguments), new[] { typeof(DependencyWithArgument).Assembly });

            help.Execute();

            Assert.Contains("DependencyArgument", this.console.Output);
            Assert.DoesNotContain("UnrelatedArgument", this.console.Output);
        }

        [Fact]
        public void ExecuteSupportsValueTypedArguments()
        {
            Help help = NewHelp(typeof(CommandWithValueTypeArgument));

            help.Execute();

            Assert.Contains("ValueTypeArgument", this.console.Output);
        }

        private static Help NewHelp(Type commandType, IEnumerable<Assembly> assemblies = null)
        {
            return new Help(commandType, assemblies ?? Enumerable.Empty<Assembly>());
        }

        [Export, Export(typeof(Command))]
        public class CommandWithArguments : Command
        {
            [Import("Argument", AllowDefault = true), Argument, Summary("Argument Summary")]
            public string Property { get; set; }

            [Import("LongArgument", AllowDefault = true), Argument, Summary("Long Argument Summary")]
            public string LongProperty { get; set; }
        }

        [Export, Export(typeof(Command)), Summary("Test command with summary")]
        public class CommandWithSummary : Command
        {
        }

        [Export, Export(typeof(Command))]
        public class CommandWithoutSummaryAndArguments : Command
        {
        }

        [Export, Export(typeof(Command))]
        public class DependencyWithArgument : Command
        {
            [Import("DependencyArgument", AllowDefault = true), Argument]
            public string DependencyProperty { get; set; }
        }

        [Export(typeof(Command))]
        public class UnrelatedWithArgument : Command
        {
            [Import("UnrelatedArgument", AllowDefault = true), Argument]
            public string UnrelatedProperty { get; set; }
        }

        [Export, Export(typeof(Command)), DependsOn(typeof(DependencyWithArgument))]
        public class DependentWithoutArguments : Command
        {
        }

        [Export, Export(typeof(Command))]
        public class CommandWithValueTypeArgument : Command
        {
            [Import("ValueTypeArgument", AllowDefault = true), Argument]
            public int ValueTypeProperty { get; set; }
        }
    }
}
