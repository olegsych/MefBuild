using System.Reflection;
using Xunit;

namespace MefBuild
{
    public static class ExecuteTest
    {
        [Fact]
        public static void ClassIsInternalAndNotMeantToBeUsedDirectly()
        {
            Assert.False(typeof(Execute).IsPublic);
        }

        [Fact]
        public static void ClassInheritsFromCommandToParticipateInBuildProcess()
        {
            Assert.True(typeof(Command).IsAssignableFrom(typeof(Execute)));
        }

        [Fact]
        public static void ClassDefinesExecutionOrderOfCommandsItDependsOn()
        {
            var metadata = typeof(Execute).GetCustomAttribute<DependsOnAttribute>();
            Assert.Equal(new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) }, metadata.Dependencies);
        }
    }
}
