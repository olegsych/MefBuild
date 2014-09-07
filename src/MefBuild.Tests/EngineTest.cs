using System;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using Moq;
using Xunit;

namespace MefBuild
{
    public class EngineTest
    {
        [Fact]
        public void ClassIsPublicAndCanBeUsedDirectly()
        {
            Assert.True(typeof(Engine).IsPublic);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            CompositionContext context = null;
            var e = Assert.Throws<ArgumentNullException>(() => new Engine(context));
            Assert.Equal("context", e.ParamName);
        }

        [Fact]
        public void ExecuteThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var engine = new Engine(new Mock<CompositionContext>().Object);
            Type commandType = null;
            var e = Assert.Throws<ArgumentNullException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
        }

        [Fact]
        public void ExecuteThrowsArgumentExceptionWhenGivenTypeIsNotCommandToPreventUsageErrors()
        {
            var engine = new Engine(new Mock<CompositionContext>().Object);
            Type commandType = typeof(object);
            var e = Assert.Throws<ArgumentException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
            Assert.Contains("Command", e.Message);
        }

        [Fact]
        public void ExecuteCreatesCommandOfGivenTypeUsingCompositionContextAndExecutesIt()
        {
            var command = new Mock<Command>(); 
            object commandObject = command.Object;

            var compositionContext = new Mock<CompositionContext>();
            compositionContext
                .Setup(m => m.TryGetExport(It.Is<CompositionContract>(c => c.ContractType == commandObject.GetType()), out commandObject))
                .Returns(true);

            var engine = new Engine(compositionContext.Object);

            engine.Execute(commandObject.GetType());

            compositionContext.Verify(m => m.TryGetExport(It.IsAny<CompositionContract>(), out commandObject));
            command.Verify(m => m.Execute());
        }
    }
}
