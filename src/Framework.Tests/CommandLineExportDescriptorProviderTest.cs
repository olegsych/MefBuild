using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using Xunit;

namespace MefBuild
{
    public static class CommandLineExportDescriptorProviderTest
    {
        [Fact]
        public static void ClassIsPublicSoThatUsersCanUseItInTheirCommandLineApplications()
        {
            Assert.True(typeof(CommandLineExportDescriptorProvider).IsPublic);
        }

        [Fact]
        public static void ClassInheritsFromExportDescriptorProviderToExtendMefContainerConfiguration()
        {
            Assert.True(typeof(ExportDescriptorProvider).IsAssignableFrom(typeof(CommandLineExportDescriptorProvider)));
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionWhenGivenArrayIsNullToPreventUsageErrors()
        {
            string[] args = null;
            var e = Assert.Throws<ArgumentNullException>(() => new CommandLineExportDescriptorProvider(args));
            Assert.Equal("commandLineArguments", e.ParamName);
        }

        public static class SatisfiesImportsWithNamedContract
        {
            [Fact]
            public static void GetExportDescriptorsSatisfiesStringImportWithNamedContract()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:TestValue" }))
                    .CreateContainer();

                var target = new Target();
                context.SatisfyImports(target);

                Assert.Equal("TestValue", target.Property);
            }

            [Fact]
            public static void ThrowsCompositionFailedExceptionSingleImportIsExpectedAndMultipleArgumentsAreSpecified()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:42", "-TestArgument:43" }))
                    .CreateContainer();

                var target = new Target();
                var e = Assert.Throws<CompositionFailedException>(() => context.SatisfyImports(target));
                Assert.Contains("command-line argument", e.Message, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("-TestArgument:42", e.Message, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("-TestArgument:43", e.Message, StringComparison.OrdinalIgnoreCase);
            }

            public class Target
            {
                [Import("TestArgument")]
                public string Property { get; set; }
            }
        }

        public static class TypeConversion
        {
            [Fact]
            public static void GetExportDescriptorConvertsArgumentValuesToExpectedTypes()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:42" }))
                    .CreateContainer();

                var target = new Target();
                context.SatisfyImports(target);

                Assert.Equal(42, target.Property);
            }

            public class Target
            {
                [Import("TestArgument")]
                public int Property { get; set; }
            }
        }

        public static class ImportMany
        {
            [Fact]
            public static void GetExportDescriptorSatisfiesImportManyWithNamedContract()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:42", "-TestArgument:43" }))
                    .CreateContainer();

                var target = new Target();
                context.SatisfyImports(target);

                Assert.Equal(new[] { 42, 43 }, target.Property);
            }

            public class Target
            {
                [ImportMany("TestArgument")]
                public IEnumerable<int> Property { get; set; }
            }
        }
    }
}
