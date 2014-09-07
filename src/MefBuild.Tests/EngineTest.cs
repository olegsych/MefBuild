using System;
using System.Composition;
using System.Composition.Hosting;
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

        public static class ExecutesEachDependsOnCommandOnce
        {
            [Fact]
            public static void ExecuteExecutesAndItsDependenciesOnceRegardlessOfHowManyTimesDependencyAppearsInTree()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Adam), typeof(Eve), typeof(Cain), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Cain));

                container.GetExport<ExecutionTracker>().Verify(typeof(Adam), typeof(Eve), typeof(Cain));
            }

            [Shared, Export]
            public class Adam : StubCommand
            {
            }

            [Shared, Export]
            public class Eve : StubCommand
            {
                [ImportingConstructor]
                public Eve(Adam adam) : base(adam)
                {
                }
            }

            [Shared, Export]
            public class Cain : StubCommand
            {
                [ImportingConstructor]
                public Cain(Adam adam, Eve eve) : base(adam, eve)
                {
                }
            }
        }

        public static class ExecutesEachBeforeCommandsOnce
        {
            [Fact]
            public static void ExecuteDoesNotExecuteBeforeCommandIfItHasAlreadyBeenExecuted()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Dependency), typeof(Target), typeof(Before), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Before), typeof(Dependency), typeof(Target));
            }

            [Shared, Export]
            public class Dependency : StubCommand
            {
            }

            [Export, Shared]
            public class Target : StubCommand
            {
                [ImportingConstructor]
                public Target(Dependency dependency) : base(dependency)
                {
                }
            }

            [Export, Shared, ExecuteBefore(typeof(Dependency)), ExecuteBefore(typeof(Target))]
            public class Before : StubCommand
            {
            }
        }

        public static class ExecutesEachAfterCommandsOnce
        {
            [Fact]
            public static void ExecuteDoesNotExecuteAfterCommandIfItHasAlreadyBeenExecuted()
            {
                CompositionContext container = new ContainerConfiguration()
                    .WithParts(typeof(Dependent), typeof(Target), typeof(After), typeof(ExecutionTracker))
                    .CreateContainer();

                new Engine(container).Execute(typeof(Target));

                container.GetExport<ExecutionTracker>().Verify(typeof(Dependent), typeof(After), typeof(Target));
            }

            [Shared, Export]
            public class Dependent : StubCommand
            {
            }

            [Shared, Export]
            public class Target : StubCommand
            {
                [ImportingConstructor]
                public Target(Dependent dependent) : base(dependent)
                {
                }
            }

            [Shared, Export, ExecuteAfter(typeof(Target)), ExecuteAfter(typeof(Dependent))]
            public class After : StubCommand
            { 
            }
        }
    }
}
