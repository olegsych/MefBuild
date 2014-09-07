using System;
using Xunit;

namespace MefBuild
{
    public class CompositeCommandTest
    {
        [Fact]
        public void ClassIsPublicForDocumentationAndExtensibility()
        {
            Assert.True(typeof(Command).IsPublic);
        }

        [Fact]
        public void ClassIsAbstractAndNotMeantToBeUsedDirectly()
        {
            Assert.True(typeof(Command).IsAbstract);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenCollectionIsNullToPreventUsageErrors()
        {
            Command[] commands = null;
            Assert.Throws<ArgumentNullException>(() => new StubCommand(commands));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenCollectionIsEmptyToPreventUsageErrors()
        {
            Assert.Throws<ArgumentException>(() => new StubCommand(new Command[0]));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenAnyCollectionElementIsNullToPreventUsageErrors()
        {
            Assert.Throws<ArgumentException>(() => new StubCommand(new Command[1]));
        }

        [Fact]
        public void ConstructorInitializesDependsOnPropertyWithGivenCollectionOfCommands()
        {
            var commands = new Command[] { new StubCommand(), new StubCommand() };
            var composite = new StubCommand(commands);
            Assert.Equal(commands, composite.DependsOn);
        }

        [Fact]
        public void ParameterlessConstructorInitializesDependsOnPropertyWithEmptyCollectionToPreventNullReferenceExceptions()
        {
            var composite = new StubCommand();
            Assert.Empty(composite.DependsOn);
        }
    }
}
