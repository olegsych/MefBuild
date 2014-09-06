using System;
using Moq;
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
        public void ClassImplementsICommandInterfaceToSupportPolymorphicExecution()
        {
            Assert.True(typeof(ICommand).IsAssignableFrom(typeof(Command)));
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
            Assert.Throws<ArgumentNullException>(() => new TestableCommand(commands));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenCollectionIsEmptyToPreventUsageErrors()
        {
            Assert.Throws<ArgumentException>(() => new TestableCommand(new Command[0]));
        }

        [Fact]
        public void ConstructorThrowsArgumentExceptionWhenAnyCollectionElementIsNullToPreventUsageErrors()
        {
            Assert.Throws<ArgumentException>(() => new TestableCommand(new Command[1]));
        }

        [Fact]
        public void ConstructorInitializesDependsOnPropertyWithGivenCollectionOfCommands()
        {
            var commands = new ICommand[] { new Mock<ICommand>().Object, new Mock<ICommand>().Object };
            var composite = new TestableCommand(commands);
            Assert.Equal(commands, composite.DependsOn);
        }

        [Fact]
        public void ParameterlessConstructorInitializesDependsOnPropertyWithEmptyCollectionToPreventNullReferenceExceptions()
        {
            var composite = new TestableCommand();
            Assert.Empty(composite.DependsOn);
        }

        private class TestableCommand : Command
        {
            public TestableCommand(params ICommand[] dependsOn) : base(dependsOn)
            {
            }

            public TestableCommand() : base()
            {
            }
        }
    }
}
