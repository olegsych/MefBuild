using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace MefBuild
{
    public static class ExecuteCommandsTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantToBeUsedDirectly()
        {
            Assert.False(typeof(ExecuteCommands).IsPublic);
        }

        [Fact]
        public static void ClassInheritsFromCommandToParticipateInBuildProcess()
        {
            Assert.True(typeof(Command).IsAssignableFrom(typeof(ExecuteCommands)));
        }

        [Fact]
        public static void ExecutesCommandOfGivenType()
        {
            TestCommand.ExecutedCommands.Clear();

            var command = new ExecuteCommands(new[] { typeof(TestCommand) }, new[] { typeof(TestCommand).Assembly });
            command.Execute();

            Assert.Equal(typeof(TestCommand), TestCommand.ExecutedCommands.Single().GetType());
        }

        [Fact]
        public static void DirectsEngineOutputToConsoleOutput()
        {
            var command = new ExecuteCommands(new[] { typeof(TestCommand) }, new[] { typeof(TestCommand).Assembly });

            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                command.Execute();
                Assert.Contains("Command \"" + typeof(TestCommand).FullName, output.ToString());
            }
        }

        [Fact]
        public static void DirectsEngineOutputToDebugOutput()
        {
            var command = new ExecuteCommands(new[] { typeof(TestCommand) }, new[] { typeof(TestCommand).Assembly });

            var output = new StringBuilder();
            using (new DebugOutputInterceptor(output))
            {
                command.Execute();
                Assert.Contains("Command \"" + typeof(TestCommand).FullName, output.ToString());
            }
        }

        [Fact]
        public static void AddsGivenAssembliesToCompositionContextToEnableDiscoveryOfExtensions()
        {
            var references = new[] 
            { 
                Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                typeof(ExecuteCommandsTest).Assembly,
                typeof(Command).Assembly,
                typeof(ExportAttribute).Assembly,
            };

            string extensionCode = @"
                using System.Composition;
                using MefBuild;

                [Command, ExecuteBefore(typeof(TestCommand))]
                public class BeforeCommand : TestCommand
                {
                }";

            Assembly extensionAssembly = CSharpCompiler.CompileInMemory(extensionCode, references);

            var command = new ExecuteCommands(
                new[] { typeof(TestCommand) },
                new[] { typeof(TestCommand).Assembly, extensionAssembly });

            TestCommand.ExecutedCommands.Clear();
            command.Execute();
            Assert.Equal(new[] { "BeforeCommand", "TestCommand" }, TestCommand.ExecutedCommands.Select(c => c.GetType().Name));
        }

        [Fact]
        public static void ExecuteOverridesInheritedMethodToBeInvokedPolymorphically()
        {
            Assert.Equal(typeof(Command).GetMethod("Execute"), typeof(ExecuteCommands).GetMethod("Execute").GetBaseDefinition());
        }

        public class RequiredExports
        {
            private readonly IEnumerable<Assembly> assemblies = Enumerable.Empty<Assembly>();
            private readonly IEnumerable<Type> commandTypes = Enumerable.Empty<Type>();

            [Export]
            public IEnumerable<Assembly> Assemblies
            {
                get { return this.assemblies; }
            }

            [Export(ContractNames.Command)]
            public IEnumerable<Type> CommandTypes
            {
                get { return this.commandTypes; }
            }
        }
    }
}
