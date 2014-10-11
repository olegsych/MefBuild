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
            Help help = NewHelp(typeof(Summarized));

            help.Execute();

            var commandAttribute = typeof(Summarized).GetCustomAttribute<CommandAttribute>();
            Assert.Contains(commandAttribute.Summary, this.console.Output);
        }

        [Fact]
        public void ExecuteDoesNotWriteBlankLineWhenCommandDoesNotHaveSummary()
        {
            Help help = NewHelp(typeof(UnsummarizedParameterless));

            help.Execute();

            Assert.StartsWith("Usage", this.console.Output);
        }

        [Fact]
        public void ExecuteWritesCommandUsageToConsoleOutput()
        {
            Help help = NewHelp(typeof(UnsummarizedParameterless));

            help.Execute();

            Assert.Contains("Usage:", this.console.Output);
            Assert.Matches(new Regex(@"^\s+MefBuild " + typeof(UnsummarizedParameterless).Name, RegexOptions.Multiline), this.console.Output);
        }

        [Fact]
        public void ExecuteWritesParameterSectionHeader()
        {
            Help help = NewHelp(typeof(Parameterized));

            help.Execute();

            Assert.Contains("Parameters:", this.console.Output);
        }

        [Fact]
        public void ExecuteDoesNotWriteParameterSectionHeaderIfCommandHasNoParameters()
        {
            Help help = NewHelp(typeof(Summarized));

            help.Execute();

            Assert.DoesNotContain("Parameters:", this.console.Output);
        }

        [Fact]
        public void ExecuteWritesParameterNamesAndSummariesToConsoleOutput()
        {
            Help help = NewHelp(typeof(Parameterized));

            help.Execute();

            var parameter = typeof(Parameterized).GetProperty("Property").GetCustomAttribute<ParameterAttribute>();
            Assert.Matches(new Regex(parameter.Name + @"\s+" + parameter.Summary), this.console.Output);
        }
        
        [Fact]
        public void ExecuteIndentsParameterNamesToImproveReadability()
        {
            Help help = NewHelp(typeof(Parameterized));

            help.Execute();

            var parameter = typeof(Parameterized).GetProperty("Property").GetCustomAttribute<ParameterAttribute>();
            Assert.Matches(new Regex(@"^\s+" + parameter.Name, RegexOptions.Multiline), this.console.Output);
        }

        [Fact]
        public void ExecuteAlignsParameterSummariesToImproveReadability()
        {
            Help help = NewHelp(typeof(Parameterized));

            help.Execute();

            var parameter = typeof(Parameterized).GetProperty("Property").GetCustomAttribute<ParameterAttribute>();
            var longParameter = typeof(Parameterized).GetProperty("LongProperty").GetCustomAttribute<ParameterAttribute>();
            Assert.Contains(parameter.Name + "     " + parameter.Summary, this.console.Output);
            Assert.Contains(longParameter.Name + " " + longParameter.Summary, this.console.Output);
        }

        [Fact]
        public void ExecuteListsParametersOfCommandsTargetCommandsDependsOn()
        {
            Help help = NewHelp(typeof(ParameterlessDependent), new[] { typeof(ParameterizedDependency).Assembly });

            help.Execute();

            Assert.Contains("DependencyParameter", this.console.Output);
            Assert.DoesNotContain("UnrelatedParameter", this.console.Output);
        }

        [Fact]
        public void ExecuteSupportsValueTypedParameters()
        {
            Help help = NewHelp(typeof(ValueTypeParameterized));

            help.Execute();

            Assert.Contains("ValueTypeParameter", this.console.Output);
        }

        private static Help NewHelp(Type commandType, IEnumerable<Assembly> assemblies = null)
        {
            return new Help(commandType, assemblies ?? Enumerable.Empty<Assembly>());
        }

        [Command]
        public class Parameterized : Command
        {
            [Import(AllowDefault = true), Parameter(Name = "Parameter", Summary = "Parameter Summary")]
            public string Property { get; set; }

            [Import(AllowDefault = true), Parameter(Name = "LongParameter", Summary = "Long Parameter Summary")]
            public string LongProperty { get; set; }
        }

        [Command(Summary = "Test command with summary")]
        public class Summarized : Command
        {
        }

        [Command]
        public class UnsummarizedParameterless : Command
        {
        }

        [Command]
        public class ParameterizedDependency : Command
        {
            [Import(AllowDefault = true), Parameter(Name = "DependencyParameter", Summary = "Dependency Parameter Summary")]
            public string DependencyProperty { get; set; }
        }

        [Command]
        public class ParameterizedUnrelated : Command
        {
            [Import(AllowDefault = true), Parameter(Name = "UnrelatedParameter", Summary = "Unrelated Parameter Summary")]
            public string UnrelatedProperty { get; set; }
        }

        [Command(DependsOn = new[] { typeof(ParameterizedDependency) })]
        public class ParameterlessDependent : Command
        {
        }

        [Command]
        public class ValueTypeParameterized : Command
        {
            [Import(AllowDefault = true), Parameter(Name = "ValueTypeParameter", Summary = "Value Type Parameter")]
            public int ValueTypeProperty { get; set; }
        }
    }
}
