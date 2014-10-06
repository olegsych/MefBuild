using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using MefBuild.Diagnostics;
using Xunit;
using Record = MefBuild.Diagnostics.Record;

namespace MefBuild
{
    public class EngineTest : IDisposable
    {
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            StubCommand.ExecutedCommands.Clear();
        }

        [Fact]
        public static void ClassIsPublicAndCanBeUsedDirectly()
        {
            Assert.True(typeof(Engine).IsPublic);
        }

        [Fact]
        public static void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            ContainerConfiguration configuration = null;
            var e = Assert.Throws<ArgumentNullException>(() => new Engine(configuration));
            Assert.Equal("configuration", e.ParamName);
        }

        [Fact]
        public static void ExecuteThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var engine = new Engine(new ContainerConfiguration());
            Type commandType = null;
            var e = Assert.Throws<ArgumentNullException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
        }

        [Fact]
        public static void ExecuteThrowsArgumentExceptionWhenGivenTypeIsNotCommandToPreventUsageErrors()
        {
            var engine = new Engine(new ContainerConfiguration());
            Type commandType = typeof(object);
            var e = Assert.Throws<ArgumentException>(() => engine.Execute(commandType));
            Assert.Equal("commandType", e.ParamName);
            Assert.Contains("Command", e.Message);
        }

        [Fact]
        public static void ExecuteConstraintsGenericTypeToICommandToPreventUsageErrors()
        {
            MethodInfo executeOfT = typeof(Engine).GetMethods().Single(m => m.Name == "Execute" && m.IsGenericMethodDefinition);
            Assert.True(typeof(Command).IsAssignableFrom(executeOfT.GetGenericArguments()[0]));
        }

        private static void InterceptLogOutput(Engine engine, ICollection<Record> records)
        {
            var output = new StubOutput();
            output.Verbosity = Verbosity.Diagnostic;
            output.OnWrite = record => records.Add(record);
            engine.Log = new Log(output);
        }

        public static class LogProperty
        {
            [Fact(Skip = "Log property should not be public")]
            public static void IsAutomaticallyImportedFromCompositionContext()
            {
                var configuration = new ContainerConfiguration().WithPart<Log>();
                var log = configuration.CreateContainer().GetExport<Log>();
                var engine = new Engine(configuration);
                Assert.Same(log, engine.Log);
            }

            [Fact]
            public static void HasDefaultValueSoThatUsersDontHaveToExportLogInCompositionContext()
            {
                var engine = new Engine(new ContainerConfiguration());
                Assert.Same(Log.Empty, engine.Log);
            }

            [Fact]
            public static void ThrowsArgumentNullExceptionToPreventUsageErrors()
            {
                var engine = new Engine(new ContainerConfiguration());
                var e = Assert.Throws<ArgumentNullException>(() => engine.Log = null);
                Assert.Equal("value", e.ParamName);
            }
        }

        public class ExecutesOne : EngineTest
        {
            [Fact]
            public void ExecutesCommandSpecifiedAsType()
            {
                var configuration = new ContainerConfiguration().WithPart<Target>();

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(new[] { typeof(Target) }, StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void ExecutesCommandSpecifiedAsGeneric()
            {
                var configuration = new ContainerConfiguration().WithPart<Target>();

                new Engine(configuration).Execute<Target>();

                Assert.Equal(new[] { typeof(Target) }, StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void ThrowsArgumentExceptionIfCommandTypeIsNotExported()
            {
                var configuration = new ContainerConfiguration();

                var engine = new Engine(configuration);
                Assert.Throws<ArgumentException>(() => engine.Execute(typeof(Target)));
            }

            [Fact]
            public void LogsStartRecordBeforeExecutingCommand()
            {
                var configuration = new ContainerConfiguration().WithPart<Target>();
                var engine = new Engine(configuration);

                var records = new List<Record>();
                EngineTest.InterceptLogOutput(engine, records);

                engine.Execute<Target>();

                var expected = new Record(
                    string.Format("Command \"{0}\" in \"{1}\":", typeof(Target).FullName, typeof(Target).Assembly.Location), 
                    RecordType.Start, 
                    Importance.High);
                Assert.Contains(expected, records);
            }

            [Fact]
            public void LogsStopRecordAfterExecutingCommand()
            {
                var configuration = new ContainerConfiguration().WithPart<Target>();
                var engine = new Engine(configuration);

                var records = new List<Record>();
                EngineTest.InterceptLogOutput(engine, records);

                engine.Execute<Target>();

                var expected = new Record(
                    string.Format("Done executing command \"{0}\".", typeof(Target).FullName), 
                    RecordType.Stop, 
                    Importance.Normal);
                Assert.Contains(expected, records);                
            }

            [Command]
            public class Target : StubCommand
            {
            }
        }

        public class ExecutesDependcies : EngineTest
        {
            [Fact]
            public void ExecutesCommandTypesSpecifiedInDependsOnAttributeBeforeCommand()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Child), typeof(Parent1), typeof(Parent2));

                new Engine(configuration).Execute(typeof(Child));

                Assert.Equal(
                    new[] { typeof(Parent1), typeof(Parent2), typeof(Child) }, 
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Command(DependsOn = new[] { typeof(Parent1), typeof(Parent2) })]
            public class Child : StubCommand
            {
            }

            [Command]
            public class Parent1 : StubCommand
            {
            }

            [Command]
            public class Parent2 : StubCommand
            {
            }
        }

        public class ExecutesDependenciesOnce : EngineTest
        {
            [Fact]
            public void ExecutesDependencyCommandMarkedWithSharedAttributeOnlyOnce()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(SharedDependency), typeof(Target));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(SharedDependency), typeof(Target) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }
            
            [Command]
            public class SharedDependency : StubCommand
            {
            }

            [Command(DependsOn = new[] { typeof(SharedDependency), typeof(SharedDependency) })]
            public class Target : StubCommand
            {
            }
        }

        public class ExecutesBeforeCommands : EngineTest
        {
            [Fact]
            public void ExecutesCommandsWithExecuteBeforeAttributeThatSpecifiesGivenCommandType()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Before1), typeof(Before2), typeof(Target));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Before1), typeof(Before2), typeof(Target) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Command(ExecuteBefore = new[] { typeof(Target) })]
            public class Before1 : StubCommand
            {
            }

            [Command(ExecuteBefore = new[] { typeof(Target) })]
            public class Before2 : StubCommand
            {
            }

            [Command]
            public class Target : StubCommand
            {
            }
        }

        public class ExecutesAfterCommands : EngineTest
        {
            [Fact]
            public void ExecuteDoesNotExecuteAfterCommandIfItHasAlreadyBeenExecuted()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(After1), typeof(After2));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Target), typeof(After1), typeof(After2) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Command]
            public class Target : StubCommand
            {
            }

            [Command(ExecuteAfter = new[] { typeof(Target) })]
            public class After1 : StubCommand
            {
            }

            [Command(ExecuteAfter = new[] { typeof(Target) })]
            public class After2 : StubCommand
            { 
            }
        }

        public class ExecutesDependenciesOfBeforeCommands : EngineTest
        {
            [Fact]
            public void ExecutesCommandsListedInDependsOnAttributeOfCommandWithExecuteBeforeAttribute()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(Before), typeof(Dependency));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Dependency), typeof(Before), typeof(Target) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Command]
            public class Target : StubCommand
            {
            }

            [Command(DependsOn = new[] { typeof(Dependency) }, ExecuteBefore = new[] { typeof(Target) })]
            public class Before : StubCommand
            {
            }

            [Command]
            public class Dependency : StubCommand
            {
            }
        }

        public class ExecutesDependenciesOfAfterCommands : EngineTest
        {
            [Fact]
            public void ExecutesCommandsListedInDependsOnAttributeOfCommandWithExecuteBeforeAttribute()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(After), typeof(Dependency));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Target), typeof(Dependency), typeof(After) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Command]
            public class Target : StubCommand
            {
            }

            [Command(DependsOn = new[] { typeof(Dependency) }, ExecuteAfter = new[] { typeof(Target) })]
            public class After : StubCommand
            {
            }

            [Command]
            public class Dependency : StubCommand
            {
            }
        }

        public class PassesExportsOfDependencyCommandToImportsOfLaterCommand : EngineTest
        {
            [Fact]
            public void ExecutePassesExportsOfDependencyCommandToImportsOfLaterCommand()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Producer), typeof(Consumer));

                new Engine(configuration).Execute<Consumer>();

                var producer = StubCommand.ExecutedCommands.OfType<Producer>().Single();
                var consumer = StubCommand.ExecutedCommands.OfType<Consumer>().Single();
                Assert.Same(producer.Export, consumer.Import);
            }

            [Command]
            public class Producer : StubCommand
            {
                [Export("DependsOnExport")]
                public object Export { get; set; }

                public override void Execute()
                {
                    base.Execute();
                    this.Export = new object();
                }
            }

            [Command(DependsOn = new[] { typeof(Producer) })]
            public class Consumer : StubCommand
            {
                [Import("DependsOnExport")]
                public object Import { get; set; }
            }
        }

        public class PassesExportsOfBeforeCommandToImportsOfLaterCommand : EngineTest
        {
            [Fact]
            public void ExecutePassesExportsOfBeforeCommandToImportsOfLaterCommand()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Producer), typeof(Consumer));

                new Engine(configuration).Execute<Consumer>();

                var producer = StubCommand.ExecutedCommands.OfType<Producer>().Single();
                var consumer = StubCommand.ExecutedCommands.OfType<Consumer>().Single();
                Assert.Same(producer.Export, consumer.Import);
            }

            [Command(ExecuteBefore = new[] { typeof(Consumer) })]
            public class Producer : StubCommand
            {
                [Export("BeforeExport")]
                public object Export { get; set; }

                public override void Execute()
                {
                    base.Execute();
                    this.Export = new object();
                }
            }

            [Command]
            public class Consumer : StubCommand
            {
                [Import("BeforeExport")]
                public object Import { get; set; }
            }
        }
    }
}
