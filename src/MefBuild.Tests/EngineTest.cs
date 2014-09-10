using System;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MefBuild
{
    public static class EngineTest
    {
        [Fact]
        public static void ClassIsPublicAndCanBeUsedDirectly()
        {
            Assert.True(typeof(Engine).IsPublic);
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            CompositionContext context = null;
            var e = Assert.Throws<ArgumentNullException>(() => new Engine(context));
            Assert.Equal("context", e.ParamName);
        }

        [Fact]
        public static void ExecuteThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var engine = new Engine(new ContainerConfiguration().CreateContainer());
            Type commandType = null;
            var e = Assert.Throws<ArgumentNullException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
        }

        [Fact]
        public static void ExecuteThrowsArgumentExceptionWhenGivenTypeIsNotCommandToPreventUsageErrors()
        {
            var engine = new Engine(new ContainerConfiguration().CreateContainer());
            Type commandType = typeof(object);
            var e = Assert.Throws<ArgumentException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
            Assert.Contains("Command", e.Message);
        }

        [Fact]
        public static void ExecuteConstraintsGenericTypeToICommandToPreventUsageErrors()
        {
            MethodInfo executeOfT = typeof(Engine).GetMethods().Single(m => m.Name == "Execute" && m.IsGenericMethodDefinition);
            Assert.True(typeof(ICommand).IsAssignableFrom(executeOfT.GetGenericArguments()[0]));
        }

        public static class ExecuteOne
        {
            [Fact]
            public static void ExecutesCommandSpecifiedAsType()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Target));
            }

            [Fact]
            public static void ExecutesCommandSpecifiedAsGeneric()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute<Target>();

                container.GetExport<ExecutionTracker>().Verify(typeof(Target));
            }

            [Fact]
            public static void ThrowsCompositionFailedExceptionIfCommandTypeIsNotExported()
            {
                CompositionContext container = new ContainerConfiguration().CreateContainer();

                var engine = new Engine(container);
                Assert.Throws<CompositionFailedException>(() => engine.Execute(typeof(Target)));
            }

            [Export]
            public class Target : StubCommand
            {
            }
        }

        public static class ExecutesDependcies
        {
            [Fact]
            public static void ExecutesCommandTypesSpecifiedInDependsOnAttributeBeforeCommand()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Child), typeof(Parent1), typeof(Parent2), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Child));

                container.GetExport<ExecutionTracker>().Verify(typeof(Parent1), typeof(Parent2), typeof(Child));
            }

            [Export, DependsOn(typeof(Parent1), typeof(Parent2))]
            public class Child : StubCommand
            {
            }

            [Export]
            public class Parent1 : StubCommand
            {
            }

            [Export]
            public class Parent2 : StubCommand
            {
            }
        }

        public static class ExecutesSharedDependenciesOnce
        {
            [Fact]
            public static void ExecutesDependencyCommandMarkedWithSharedAttributeOnlyOnce()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(SharedDependency), typeof(Target), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(SharedDependency), typeof(Target));
            }
            
            [Shared, Export]
            public class SharedDependency : StubCommand
            {
            }

            [Export, DependsOn(typeof(SharedDependency), typeof(SharedDependency))]
            public class Target : StubCommand
            {
            }
        }

        public static class ExecutesNewInstancesOfNonSharedDependencies
        {
            [Fact]
            public static void ExecutesDependencyCommandNotMarkedWithSharedAttributeAsManyTimesAsItAppearsInMetadata()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Dependency), typeof(Target), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Dependency), typeof(Dependency), typeof(Target));
            }

            [Export]
            public class Dependency : StubCommand
            {
            }

            [Export, DependsOn(typeof(Dependency), typeof(Dependency))]
            public class Target : StubCommand
            {
            }
        }

        public static class ExecuteBeforeCommands
        {
            [Fact]
            public static void ExecutesCommandsWithExecuteBeforeAttributeThatSpecifiesGivenCommandType()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Before1), typeof(Before2), typeof(Target), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Before1), typeof(Before2), typeof(Target));
            }

            [Export, ExecuteBefore(typeof(Target))]
            public class Before1 : StubCommand
            {
            }

            [Export, ExecuteBefore(typeof(Target))]
            public class Before2 : StubCommand
            {
            }

            [Export]
            public class Target : StubCommand
            {
            }
        }

        public static class ExecutesAfterCommands
        {
            [Fact]
            public static void ExecuteDoesNotExecuteAfterCommandIfItHasAlreadyBeenExecuted()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(After1), typeof(After2), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Target), typeof(After1), typeof(After2));
            }

            [Export]
            public class Target : StubCommand
            {
            }

            [Export, ExecuteAfter(typeof(Target))]
            public class After1 : StubCommand
            {
            }

            [Export, ExecuteAfter(typeof(Target))]
            public class After2 : StubCommand
            { 
            }
        }

        public static class ExecutesDependenciesOfBeforeCommands
        {
            [Fact]
            public static void ExecutesCommandsListedInDependsOnAttributeOfCommandWithExecuteBeforeAttribute()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(Before), typeof(Dependency), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Dependency), typeof(Before), typeof(Target));
            }

            [Export]
            public class Target : StubCommand
            {
            }

            [Export, DependsOn(typeof(Dependency)), ExecuteBefore(typeof(Target))]
            public class Before : StubCommand
            {
            }

            [Export]
            public class Dependency : StubCommand
            {
            }
        }

        public static class ExecutesDependenciesOfAfterCommands
        {
            [Fact]
            public static void ExecutesCommandsListedInDependsOnAttributeOfCommandWithExecuteBeforeAttribute()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(After), typeof(Dependency), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Target), typeof(Dependency), typeof(After));
            }

            [Export]
            public class Target : StubCommand
            {
            }

            [Export, DependsOn(typeof(Dependency)), ExecuteAfter(typeof(Target))]
            public class After : StubCommand
            {
            }

            [Export]
            public class Dependency : StubCommand
            {
            }
        }
    }
}
