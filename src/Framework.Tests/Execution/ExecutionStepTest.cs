using System;
using Xunit;

namespace MefBuild.Execution
{
    public class ExecutionStepTest
    {
        [Fact]
        public void ConstructorInitializesPropertiesWithGivenValues()
        {
            var target = new Lazy<Command, CommandMetadata>(null);
            var dependencyType = DependencyType.DependsOn;
            var dependency = new Lazy<Command, CommandMetadata>(null);

            var step = new ExecutionStep(target, dependencyType, dependency);

            Assert.Same(target, step.Command);
            Assert.Equal(dependencyType, step.DependencyType);
            Assert.Same(dependency, step.Dependency);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenTargetCommandIsNullToPreventUsageErrors()
        {
            var dependencyType = default(DependencyType);
            var dependency = new Lazy<Command, CommandMetadata>(null);
            var e = Assert.Throws<ArgumentNullException>(() => new ExecutionStep(null, dependencyType, dependency));
            Assert.Equal("command", e.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenDependencyIsNullToPreventUsageErrors()
        {
            var command = new Lazy<Command, CommandMetadata>(null);
            var dependencyType = DependencyType.DependsOn;
            var e = Assert.Throws<ArgumentNullException>(() => new ExecutionStep(command, dependencyType, null));
            Assert.Equal("dependency", e.ParamName);
        }

        [Fact]
        public void ConstructorDoesNotThrowArgumentNullExceptionWhenDependencyTypeIsNoneBecauseDependencyIsExpectedToBeNull()
        {
            var step = new ExecutionStep(new Lazy<Command, CommandMetadata>(null), DependencyType.None, null);
            Assert.Null(step.Dependency);
        }

        [Fact]
        public void PropertiesAreReadonlyBecauseInstanceIsImmutable()
        {
            Assert.Null(typeof(ExecutionStep).GetProperty("Command").SetMethod);
            Assert.Null(typeof(ExecutionStep).GetProperty("DependencyType").SetMethod);
            Assert.Null(typeof(ExecutionStep).GetProperty("Dependency").SetMethod);
        }
    }
}
