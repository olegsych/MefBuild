using System.Composition;
using System.Composition.Hosting;
using System.Linq;
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
        public static void ClassIsExportedSharedToExecuteOnlyOnceDuringBuild()
        {
            CompositionContext context = new ContainerConfiguration().WithPart<Execute>().CreateContainer();
            var instance1 = context.GetExport<Execute>();
            var instance2 = context.GetExport<Execute>();
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public static void ClassDefinesExecutionOrderOfCommandsItDependsOn()
        {
            DependsOnAttribute attribute = typeof(Execute).GetCustomAttributes(false).OfType<DependsOnAttribute>().Single();
            Assert.Equal(new[] { typeof(LoadAssemblies), typeof(ResolveCommandTypes), typeof(ExecuteCommands) }, attribute.DependsOn);
        }
    }
}
