using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class ExecuteAttributeTest
    {
        [Fact]
        public void ClassIsPublicBecauseItHasPublicDerivedClasses()
        {
            Assert.True(typeof(ExecuteAttribute).IsPublic);
        }

        [Fact]
        public void ClassIsAbstractAndMeantToServeOnlyAsBaseClass()
        {
            Assert.True(typeof(ExecuteAttribute).IsAbstract);
        }

        [Fact]
        public void ClassInheritsFromExportAttributeToBeRecognizedByMef()
        {
            Assert.True(typeof(ExportAttribute).IsAssignableFrom(typeof(ExecuteAttribute)));
        }

        [Fact]
        public void ConstructorSetsContractTypeToICommand()
        {
            var attribute = new TestableExecuteAttribute(string.Empty, typeof(StubCommand));
            Assert.Equal(typeof(Command), attribute.ContractType);
        }

        [Fact]
        public void ConstructorSetsContractNameToGivenPrefixCombinedWithFullNameOfTargetCommandType()
        {
            var attribute = new TestableExecuteAttribute("Prefix.", typeof(Command));
            Assert.Equal("Prefix." + typeof(Command).FullName, attribute.ContractName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenContractNamePrefixIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TestableExecuteAttribute(null, typeof(Command)));
            Assert.Equal("contractNamePrefix", e.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenTargetCommandTypeIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new TestableExecuteAttribute("Prefix.", null));
            Assert.Equal("targetCommandType", e.ParamName);
        }

        [Fact]
        public void TargetCommandTypeIsInitializedByConstructor()
        {
            var e = new TestableExecuteAttribute("Prefix.", typeof(Command));
            Type targetCommandType = e.TargetCommandType;
            Assert.Equal(typeof(Command), targetCommandType);
        }

        [Fact]
        public void TargetCommandTypeIsReadOnlyBecauseItIsAlwaysInitializedByConstructor()
        {
            Assert.Null(typeof(ExecuteAttribute).GetProperty("TargetCommandType").SetMethod);
        }

        [Fact]
        public void AttributeMakesClassesItIsAppliedToDiscoverableThroughCompositionContext()
        {
            var configuration = new ContainerConfiguration()
                .WithPart<TestCommand1>()
                .WithPart<TestCommand2>();
            CompositionHost container = configuration.CreateContainer();

            IEnumerable<Command> exports = container.GetExports<Command>("Prefix" + typeof(StubCommand).FullName);

            Assert.Equal(2, exports.Count());
            Assert.Equal(typeof(TestCommand1), exports.First().GetType());
            Assert.Equal(typeof(TestCommand2), exports.Last().GetType());
        }

        [TestableExecute("Prefix", typeof(StubCommand))]
        private class TestCommand1 : Command
        {
        }

        [TestableExecute("Prefix", typeof(StubCommand))]
        private class TestCommand2 : Command
        {
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private class TestableExecuteAttribute : ExecuteAttribute
        {
            public TestableExecuteAttribute(string contractPrefix, Type targetCommand) : base(contractPrefix, targetCommand)
            {
            }
        }
    }
}
