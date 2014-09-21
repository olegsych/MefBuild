using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Xunit;

namespace MefBuild
{
    public static class ResolveCommandTypesTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantToBeConsumedDirectly()
        {
            Assert.False(typeof(ResolveCommandTypes).IsPublic);
        }

        [Fact]
        public static void ClassInheritsFromCommandToParticipateInBuildProcess()
        {
            Assert.True(typeof(Command).IsAssignableFrom(typeof(ResolveCommandTypes)));
        }

        [Fact]
        public static void ClassIsExportedSharedToPreventCreationOfMoreThanOneInstanceDuringComposition()
        {
            CompositionContext context = new ContainerConfiguration()
                .WithParts(typeof(ResolveCommandTypes), typeof(RequiredExports))
                .CreateContainer();
            var instance1 = context.GetExport<ResolveCommandTypes>();
            var instance2 = context.GetExport<ResolveCommandTypes>();
            Assert.Same(instance1, instance2);
        }

        private static Assembly Compile(string code)
        {
            var options = new CompilerParameters();
            options.GenerateInMemory = true;

            var codeProvider = new CSharpCodeProvider();
            CompilerResults results = codeProvider.CompileAssemblyFromSource(options, code);

            return results.CompiledAssembly;
        }

        public class RequiredExports
        {
            [Export]
            public IEnumerable<Assembly> Assemblies 
            {
                get { return Enumerable.Empty<Assembly>(); } 
            }
        }

        public static class CommandTypeNames
        {
            [Fact]
            public static void AreImportedDuringCompositionBasedOnCommandLineArguments()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithParts(typeof(ResolveCommandTypes), typeof(CommandLineArguments), typeof(RequiredExports))
                    .CreateContainer();

                var command = context.GetExport<ResolveCommandTypes>();

                Assert.Equal(new[] { "TestCommand1", "TestCommand2" }, command.CommandTypeNames);
            }

            public class CommandLineArguments
            {
                [Export(ContractNames.Command)]
                public string Command1 
                { 
                    get { return "TestCommand1"; } 
                }

                [Export(ContractNames.Command)]
                public string Command2 
                { 
                    get { return "TestCommand2"; } 
                }
            }
        }

        public static class CommandTypes
        {
            [Fact]
            public static void PropertyIsExportedDuringCompositionForNextCommandInBuildProcess()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithParts(typeof(ResolveCommandTypes), typeof(RequiredExports))
                    .CreateContainer();

                var command = context.GetExport<ResolveCommandTypes>();
                var commandTypes = context.GetExport<IEnumerable<Type>>(ContractNames.Command);

                Assert.NotNull(commandTypes);
                Assert.Same(command.CommandTypes, commandTypes);
            }

            [Fact]
            public static void PropertyIsReadOnlyBecauseItIsOutputOnly()
            {
                Assert.Null(typeof(ResolveCommandTypes).GetProperty("CommandTypes").SetMethod);
            }
        }

        public static class Assemblies
        {
            [Fact]
            public static void AreImportedDuringCompositionFromPreviousCommand()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithParts(typeof(ResolveCommandTypes), typeof(PreviousCommand))
                    .CreateContainer();

                var command = context.GetExport<ResolveCommandTypes>();

                Assert.Equal(new PreviousCommand().Assemblies, command.Assemblies);
            }

            public class PreviousCommand
            {
                [Export]
                public IEnumerable<Assembly> Assemblies
                {
                    get { return new[] { typeof(Command).Assembly, this.GetType().Assembly }; }
                }
            }
        }

        public static class Execute
        {
            [Fact]
            public static void ProbesAssembliesToResolveFullTypeNames()
            {
                Assembly emptyAssembly = Compile(@"namespace EmptyNamespace {}");
                Assembly testAssembly = Compile(@"
                    namespace TestNamespace {
                        public class TestClass {}
                    }");

                var command = new ResolveCommandTypes();
                command.Assemblies = new[] { emptyAssembly, testAssembly };
                command.CommandTypeNames = new[] { "TestNamespace.TestClass" };
                command.Execute();

                Assert.Equal(new[] { testAssembly.GetType("TestNamespace.TestClass") }, command.CommandTypes);
            }

            [Fact]
            public static void ThrowsExceptionWhenCommandTypeNameCannotBeResolved()
            {
                var command = new ResolveCommandTypes();
                command.Assemblies = Enumerable.Empty<Assembly>();
                command.CommandTypeNames = new[] { "MissingCommandType" };

                var e = Assert.ThrowsAny<Exception>(() => command.Execute());
                Assert.Contains("MissingCommandType", e.Message);
            }
        }
    }
}
