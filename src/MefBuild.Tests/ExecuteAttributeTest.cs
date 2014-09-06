using System;
using System.Composition;
using System.Linq;
using Moq;
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
            var attribute = new TestableExecuteAttribute(string.Empty, typeof(Mock<Command>));
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

        private class TestableExecuteAttribute : ExecuteAttribute
        {
            public TestableExecuteAttribute(string contractPrefix, Type targetCommand) : base(contractPrefix, targetCommand)
            {
            }
        }
    }
}
