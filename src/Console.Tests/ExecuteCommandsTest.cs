using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
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
        public static void AssembliesPropertyIsImportedDuringComposition()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithParts(typeof(ExecuteCommands), typeof(RequiredExports))
                .CreateContainer();

            var assemblies = context.GetExport<IEnumerable<Assembly>>();
            var command = context.GetExport<ExecuteCommands>();

            Assert.Same(assemblies, command.Assemblies);
        }

        [Fact]
        public static void CommandTypesPropertyIsImportedDuringComposition()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithDefaultConventions(new CommandExportConventions())
                .WithParts(typeof(ExecuteCommands), typeof(RequiredExports))
                .CreateContainer();

            var commandTypes = context.GetExport<IEnumerable<Type>>(ContractNames.Command);
            var command = context.GetExport<ExecuteCommands>();

            Assert.Same(commandTypes, command.CommandTypes);
        }

        [Fact]
        public static void ExecutesCommandOfGivenTypeInCompositionContext()
        {
            TestCommand.ExecutedCommands.Clear();

            var command = new ExecuteCommands();
            command.Assemblies = new[] { typeof(TestCommand).Assembly };
            command.CommandTypes = new[] { typeof(TestCommand) };
            command.Execute();

            Assert.Equal(typeof(TestCommand), TestCommand.ExecutedCommands.Single().GetType());
        }

        [Fact]
        public static void DirectsEngineOutputToConsoleOutput()
        {
            var command = new ExecuteCommands();
            command.Assemblies = new[] { typeof(TestCommand).Assembly };
            command.CommandTypes = new[] { typeof(TestCommand) };

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
            var command = new ExecuteCommands();
            command.Assemblies = new[] { typeof(TestCommand).Assembly };
            command.CommandTypes = new[] { typeof(TestCommand) };

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

                [Command(ExecuteBefore = new[] { typeof(TestCommand) })]
                public class BeforeCommand : TestCommand
                {
                }";

            Assembly extensionAssembly = CSharpCompiler.CompileInMemory(extensionCode, references);

            var command = new ExecuteCommands();
            command.Assemblies = new[] { typeof(TestCommand).Assembly, extensionAssembly };
            command.CommandTypes = new[] { typeof(TestCommand) };

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
