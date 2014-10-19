using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using Microsoft.CSharp;
using Xunit;

namespace MefBuild
{
    public static class LoadAssembliesTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantToBeUsedDirectly()
        {
            Assert.False(typeof(LoadAssemblies).IsPublic);
        }

        [Fact]
        public static void ClassInheritsFromCommandToParticipateInBuildProcess()
        {
            Assert.True(typeof(Command).IsAssignableFrom(typeof(LoadAssemblies)));
        }

        private static Assembly CreateTestAssembly(string fileName)
        {
            var options = new CompilerParameters(new string[0], fileName);

            var codeProvider = new CSharpCodeProvider();
            CompilerResults results = codeProvider.CompileAssemblyFromSource(options);

            return results.CompiledAssembly;
        }

        public static class Assemblies
        {
            [Fact]
            public static void PropertyIsExportedForNextCommandInBuildProcess()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithDefaultConventions(new CommandExportConventions())
                    .WithPart<LoadAssemblies>()
                    .CreateContainer();

                var assemblies = context.GetExport<IEnumerable<Assembly>>();
                var command = context.GetExport<LoadAssemblies>();

                Assert.NotNull(assemblies);
                Assert.Same(assemblies, command.Assemblies);
            }

            [Fact]
            public static void PropertyIsReadOnlyBecauseItIsOutputOnly()
            {
                Assert.Null(typeof(LoadAssemblies).GetProperty("Assemblies").SetMethod);
            }
        }

        public static class Execute
        {
            [Fact]
            public static void LoadsAssembliesWithGivenFileNames()
            {
                Assembly testAssembly1 = CreateTestAssembly("TestAssembly1.dll");
                Assembly testAssembly2 = CreateTestAssembly("TestAssembly2.dll");

                var command = new LoadAssemblies(new[] { testAssembly1.Location, testAssembly2.Location });
                command.Execute();

                Assert.Equal(new[] { testAssembly1, testAssembly2 }, command.Assemblies);
            }

            [Fact]
            public static void ThrowsExceptionWhenAssemblyCouldNotBeLoadedFromGivenFileName()
            {
                var command = new LoadAssemblies(new[] { "NonExistentAssembly.dll" });
                var e = Assert.ThrowsAny<Exception>(() => command.Execute());
                Assert.Contains("NonExistentAssembly.dll", e.Message);
            }

            [Fact]
            public static void OverridesMethodInheritedFromBaseClassToBeIvokedPolymorphically()
            {
                Assert.Equal(typeof(Command).GetMethod("Execute"), typeof(LoadAssemblies).GetMethod("Execute").GetBaseDefinition());
            }
        }
    }
}
