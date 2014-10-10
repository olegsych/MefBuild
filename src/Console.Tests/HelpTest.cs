using System;
using System.Composition;
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
            var help = new Help(typeof(TestCommand));

            help.Execute();

            var commandAttribute = typeof(TestCommand).GetCustomAttribute<CommandAttribute>();
            Assert.Contains(commandAttribute.Summary, this.console.Output);
        }

        [Fact]
        public void ExecuteDoesNotWriteBlankLineWhenCommandDoesNotHaveSummary()
        {
            var help = new Help(typeof(CommandWithoutSummary));

            help.Execute();

            Assert.StartsWith("Usage", this.console.Output);
        }

        [Fact]
        public void ExecuteWritesCommandUsageToConsoleOutput()
        {
            var help = new Help(typeof(TestCommand));

            help.Execute();

            Assert.Contains("Usage:", this.console.Output);
            Assert.Matches(new Regex(@"^\s+MefBuild " + typeof(TestCommand).Name, RegexOptions.Multiline), this.console.Output);
        }

        [Fact]
        public void ExecuteWritesParameterSectionHeader()
        {
            var help = new Help(typeof(TestCommand));

            help.Execute();

            Assert.Contains("Parameters:", this.console.Output);
        }

        [Fact]
        public void ExecuteDoesNotWriteParameterSectionHeaderIfCommandHasNoParameters()
        {
            var help = new Help(typeof(CommandWithoutParameters));

            help.Execute();

            Assert.DoesNotContain("Parameters:", this.console.Output);
        }

        [Fact]
        public void ExecuteWritesParameterNamesAndSummariesToConsoleOutput()
        {
            var help = new Help(typeof(TestCommand));

            help.Execute();

            var parameter = typeof(TestCommand).GetProperty("IntProperty").GetCustomAttribute<ParameterAttribute>();
            Assert.Matches(new Regex(parameter.Name + @"\s+" + parameter.Summary), this.console.Output);
        }
        
        [Fact]
        public void ExecuteIndentsParameterNamesToImproveReadability()
        {
            var help = new Help(typeof(TestCommand));

            help.Execute();

            var parameter = typeof(TestCommand).GetProperty("IntProperty").GetCustomAttribute<ParameterAttribute>();
            Assert.Matches(new Regex(@"^\s+" + parameter.Name, RegexOptions.Multiline), this.console.Output);
        }

        [Fact]
        public void ExecuteAlignsParameterSummariesToImproveReadability()
        {
            var help = new Help(typeof(TestCommand));

            help.Execute();

            var intParameter = typeof(TestCommand).GetProperty("IntProperty").GetCustomAttribute<ParameterAttribute>();
            var stringParameter = typeof(TestCommand).GetProperty("StringProperty").GetCustomAttribute<ParameterAttribute>();
            Assert.Contains(intParameter.Name + "    " + intParameter.Summary, this.console.Output);
            Assert.Contains(stringParameter.Name + " " + stringParameter.Summary, this.console.Output);
        }

        [Fact(Skip = "Until Engine is modified to support this feature of Help command")]
        public void ExecuteListsParametersOfCommandsTargetCommandsDependsOn()
        {
            var help = new Help(typeof(ParameterlessCommandWithDependency));

            help.Execute();

            Assert.Contains("Parameters:", this.console.Output);
        }

        [Command(Summary = "Helps testing the Help command")]
        public class TestCommand : Command
        {
            [Import(AllowDefault = true), Parameter(Name = "IntParameter", Summary = "Int Parameter Summary")]
            public int IntProperty { get; set; }

            [Import(AllowDefault = true), Parameter(Name = "StringParameter", Summary = "String Parameter Summary")]
            public string StringProperty { get; set; }
        }

        [Command(Summary = "Test command without parameters")]
        public class CommandWithoutParameters : Command
        {
        }

        [Command]
        public class CommandWithoutSummary : Command
        {
        }

        [Command(DependsOn = new[] { typeof(TestCommand) })]
        public class ParameterlessCommandWithDependency : Command
        {
        }
    }
}
