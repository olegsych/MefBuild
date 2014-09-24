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

        [Export(typeof(ILog)), Shared]
        public class StubLog : ILog
        {
            public void WriteCritical(string message)
            {
            }

            public void WriteError(string message)
            {
            }

            public void WriteInformation(string message)
            {
            }

            public void WriteVerbose(string message)
            {
            }

            public void WriteWarning(string message)
            {
            }
        }
    }
}
