using System;
using System.Composition;
using System.Composition.Hosting;
using MefBuild.Hosting;
using Xunit;

namespace MefBuild
{
    public class CommandTest
    {
        [Fact]
        public void ClassIsPublicAndServesAsBaseClassForUserCommands()
        {
            Assert.True(typeof(Command).IsPublic);
        }

        [Fact]
        public void ClassIsAbstractAndNotMeantToBeInstantiatedDirectly()
        {
            Assert.True(typeof(Command).IsAbstract);
        }

        [Fact]
        public void ExecuteIsVirtualForUsersToImplementActualCommandLogic()
        {
            Assert.True(typeof(Command).GetMethod("Execute").IsVirtual);
        }

        [Fact]
        public void ExecuteIsNotAbstractSoThatUsersDontHaveToImplementItInPureGroupingCommands()
        {
            Assert.False(typeof(Command).GetMethod("Execute").IsAbstract);
        }

        [Fact]
        public void LogIsAutomaticallyImportedDuringComposition()
        {
            CompositionContext context = new ContainerConfiguration().WithPart<StubLog>().CreateContainer();
            var command = new StubCommand();
            context.SatisfyImports(command);
            Assert.IsType<StubLog>(command.Log);
        }

        [Fact]
        public void LogHasDefaultValueToEnableTestingCommandsWithoutComposition()
        {
            var command = new StubCommand();
            Assert.IsType<DefaultLog>(command.Log);
        }

        [Fact]
        public void LogSetterThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var command = new StubCommand();
            var e = Assert.Throws<ArgumentNullException>(() => command.Log = null);
            Assert.Equal("value", e.ParamName);
        }

        [Export(typeof(Log)), Shared]
        public class StubLog : Log
        {
            protected override void Write(EventType eventType, string text)
            {
                throw new NotImplementedException();
            }
        }
    }
}
