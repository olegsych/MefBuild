using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MefBuild.Hosting
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

        public static class SatisfiesSingleImportWithNamedContract
        {
            [Fact]
            public static void GetExportDescriptorsSatisfiesStringImportWithNamedContract()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/TestArgument=TestValue" }))
                    .CreateContainer();

                var target = new Target();
                context.SatisfyImports(target);

                Assert.Equal("TestValue", target.Property);
            }

            [Fact]
            public static void ThrowsCompositionFailedExceptionSingleImportIsExpectedAndMultipleArgumentsAreSpecified()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/TestArgument=42", "/TestArgument=43" }))
                    .CreateContainer();

                var target = new Target();
                var e = Assert.Throws<CompositionFailedException>(() => context.SatisfyImports(target));
                Assert.Contains("command-line argument", e.Message, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("/TestArgument=42", e.Message, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("/TestArgument=43", e.Message, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public static void IgnoresCaseWhenComparingArgumentAndContractName()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/testArgument=TestValue" }))
                    .CreateContainer();

                var target = new Target();
                context.SatisfyImports(target);

                Assert.Equal("TestValue", target.Property);
            }

            public class Target
            {
                [Import("TestArgument")]
                public string Property { get; set; }
            }
        }

        public static class ConvertsValuesFromStringToExpectedType
        {
            [Fact]
            public static void GetExportDescriptorConvertsArgumentValuesToExpectedTypes()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/TestArgument=42" }))
                    .CreateContainer();

                var target = new TargetWithInt32Property();
                context.SatisfyImports(target);

                Assert.Equal(42, target.Property);
            }

            [Fact]
            public static void GetExportDescriptorConvertsArgumentValuesToEnumTypesIgnoringCase()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/TestArgument=testValue" }))
                    .CreateContainer();

                var target = new TargetWithEnumProperty();
                context.SatisfyImports(target);

                Assert.Equal(TestEnum.TestValue, target.Property);
            }

            public class TargetWithInt32Property
            {
                [Import("TestArgument")]
                public int Property { get; set; }
            }

            public enum TestEnum
            {
                DefaultValue,
                TestValue,
            }

            public class TargetWithEnumProperty
            {
                [Import("TestArgument")]
                public TestEnum Property { get; set; }
            }
        }

        public static class SatisfiesImportManyWithNamedContract
        {
            [Fact]
            public static void GetExportDescriptorSatisfiesImportManyWithNamedContract()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "/TestArgument=42", "/TestArgument=43" }))
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

        public static class IgnoresSingleCollectionImports
        {
            [Fact]
            public static void GetExportDescriptorIgnoresIEnumerablePropertyWithSingleImportBecauseItIsNotSupported()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:42" }))
                    .CreateContainer();

                var target = new IEnumerableTarget();
                var e = Assert.Throws<CompositionFailedException>(() => context.SatisfyImports(target));
                Assert.Contains("Missing dependency 'Property'", e.Message);
            }

            [Fact]
            public static void GetExportDescriptorIgnoresIListPropertyWithSingleImportBecauseItIsNotSupported()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:42" }))
                    .CreateContainer();

                var target = new IListTarget();
                var e = Assert.Throws<CompositionFailedException>(() => context.SatisfyImports(target));
                Assert.Contains("Missing dependency 'Property'", e.Message);
            }

            [Fact]
            public static void GetExportDescriptorIgnoresArrayPropertyWithSingleImportBecauseItIsNotSupported()
            {
                CompositionContext context = new ContainerConfiguration()
                    .WithProvider(new CommandLineExportDescriptorProvider(new[] { "-TestArgument:42" }))
                    .CreateContainer();

                var target = new ArrayTarget();
                var e = Assert.Throws<CompositionFailedException>(() => context.SatisfyImports(target));
                Assert.Contains("Missing dependency 'Property'", e.Message);
            }

            public class ArrayTarget
            {
                [Import("TestArgument")]
                [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Need to test array imports")]
                public string[] Property { get; set; }
            }

            public class IEnumerableTarget
            {
                [Import("TestArgument")]
                public IEnumerable<string> Property { get; set; }
            }

            public class IListTarget
            {
                [Import("TestArgument")]
                [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need to test IList imports")]
                public IList<string> Property { get; set; }
            }
        }
    }
}
