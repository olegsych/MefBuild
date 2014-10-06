using System;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using MefBuild.Diagnostics;
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
        public void LogHasDefaultValueToEnableTestingCommandsWithoutComposition()
        {
            var command = new StubCommand();
            Assert.Same(Log.Empty, command.Log);
        }

        [Fact]
        public void LogIsProtectedBecauseItShouldBeAccessedOnlyByCommand()
        {
            Assert.False(typeof(Command).GetProperty("Log", BindingFlags.Instance | BindingFlags.NonPublic).GetMethod.IsPublic);
        }
    }
}
