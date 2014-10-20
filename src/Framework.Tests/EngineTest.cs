using System;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
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
            StubOutput.WrittenRecords.Clear();
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
        public static void ExecuteThrowsArgumentNullExceptionWhenGivenTypeIsNullToPreventUsageErrors()
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

        public class ExecutesOne : EngineTest
        {
            [Fact]
            public void ExecutesCommandSpecifiedAsType()
            {
                var configuration = new ContainerConfiguration().WithPart<TypedCommand>();

                new Engine(configuration).Execute(typeof(TypedCommand));

                Assert.Equal(new[] { typeof(TypedCommand) }, StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void ExecutesCommandSpecifiedAsGeneric()
            {
                var configuration = new ContainerConfiguration().WithPart<TypedCommand>();

                new Engine(configuration).Execute<TypedCommand>();

                Assert.Equal(new[] { typeof(TypedCommand) }, StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void ThrowsCompositionFailedExceptionIfCommandTypeIsNotExported()
            {
                var configuration = new ContainerConfiguration();
                var engine = new Engine(configuration);

                Assert.Throws<CompositionFailedException>(() => engine.Execute(typeof(TypedCommand)));
            }

            [Fact]
            public void ExecutesCommandExportedWithGivenContractName()
            {
                var configuration = new ContainerConfiguration().WithPart<NamedCommand>();

                new Engine(configuration).Execute(NamedCommandName);

                Assert.Equal(new[] { typeof(NamedCommand) }, StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void ExecuteThrowsArgumentNullExceptionWhenCommandNameIsNull()
            {
                var configuration = new ContainerConfiguration();
                var engine = new Engine(configuration);

                var e = Assert.Throws<ArgumentNullException>(() => engine.Execute((string)null));
                Assert.Equal("commandName", e.ParamName);
            }

            [Fact]
            public void ExecuteThrowsCompositionFailedExceptionIfCommandNameIsNotExported()
            {
                var configuration = new ContainerConfiguration();
                var engine = new Engine(configuration);

                Assert.Throws<CompositionFailedException>(() => engine.Execute(NamedCommandName));
            }

            [Fact]
            public void LogsStartRecordBeforeExecutingCommand()
            {
                var configuration = new ContainerConfiguration().WithParts(typeof(TypedCommand), typeof(StubOutput));
                var engine = new Engine(configuration);

                engine.Execute<TypedCommand>();

                var expected = new Record(
                    string.Format("Command \"{0}\" in \"{1}\":", typeof(TypedCommand).FullName, typeof(TypedCommand).Assembly.Location), 
                    RecordType.Start, 
                    Importance.High);
                Assert.Contains(expected, StubOutput.WrittenRecords);
            }

            [Fact]
            public void LogsStopRecordAfterExecutingCommand()
            {
                var configuration = new ContainerConfiguration().WithParts(typeof(TypedCommand), typeof(StubOutput));
                var engine = new Engine(configuration);

                engine.Execute<TypedCommand>();

                var expected = new Record(
                    string.Format("Done executing command \"{0}\".", typeof(TypedCommand).FullName), 
                    RecordType.Stop, 
                    Importance.Normal);
                Assert.Contains(expected, StubOutput.WrittenRecords);                
            }

            private const string NamedCommandName = "NamedCommand";

            [Export]
            public class TypedCommand : StubCommand
            {
            }

            [Export(NamedCommandName, typeof(Command))]
            public class NamedCommand : StubCommand
            {
            }
        }

        public class ExecutesDependcies : EngineTest
        {
            [Fact]
            public void ExecutesCommandTypesSpecifiedInDependsOnAttributeBeforeCommand()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Child), typeof(Parent));

                new Engine(configuration).Execute(typeof(Child));

                Assert.Equal(
                    new[] { typeof(Parent), typeof(Child) }, 
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void LogsStartRecordWithNameOfCommandThatDependsOnCurrentCommand()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Child), typeof(Parent), typeof(StubOutput));

                new Engine(configuration).Execute<Child>();

                Record startRecord = StubOutput.WrittenRecords.First(r => r.RecordType == RecordType.Start);
                Assert.EndsWith(
                    string.Format(" (command \"{0}\" depends on it):", typeof(Child).FullName),
                    startRecord.Text);
            }

            [Export, DependsOn(typeof(Parent))]
            public class Child : StubCommand
            {
            }

            [Export]
            public class Parent : StubCommand
            {
            }
        }

        public class ExecutesDependenciesOnce : EngineTest
        {
            [Fact]
            public void ExecutesDependencyCommandOnlyOnce()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Dependency), typeof(Target));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Dependency), typeof(Target) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
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

        public class ExecutesBeforeCommands : EngineTest
        {
            [Fact]
            public void ExecutesCommandsWithExecuteBeforeAttributeThatSpecifiesGivenCommandType()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Before), typeof(Target));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Before), typeof(Target) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void LogsStartRecordWithNameOfCommandThatCurrentCommandMustBeExecutedBefore()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Before), typeof(Target), typeof(StubOutput));

                new Engine(configuration).Execute(typeof(Target));

                var startRecord = StubOutput.WrittenRecords.First(r => r.RecordType == RecordType.Start);
                Assert.EndsWith(
                    string.Format(" (it executes before \"{0}\"):", typeof(Target).FullName), 
                    startRecord.Text);
            }

            [ExecuteBefore(typeof(Target))]
            public class Before : StubCommand
            {
            }

            [Export]
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
                    .WithParts(typeof(Target), typeof(After));

                new Engine(configuration).Execute(typeof(Target));

                Assert.Equal(
                    new[] { typeof(Target), typeof(After) },
                    StubCommand.ExecutedCommands.Select(c => c.GetType()));
            }

            [Fact]
            public void LogsStartRecordWithNameOfCommandThatCurrentCommandMustBeExecutedAfter()
            {
                var configuration = new ContainerConfiguration()
                    .WithParts(typeof(Target), typeof(After), typeof(StubOutput));

                new Engine(configuration).Execute(typeof(Target));

                var startRecord = StubOutput.WrittenRecords.Last(r => r.RecordType == RecordType.Start);
                Assert.EndsWith(
                    string.Format(" (it executes after \"{0}\"):", typeof(Target).FullName),
                    startRecord.Text);
            }

            [Export]
            public class Target : StubCommand
            {
            }

            [ExecuteAfter(typeof(Target))]
            public class After : StubCommand
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

            [Export]
            public class Target : StubCommand
            {
            }

            [DependsOn(typeof(Dependency)), ExecuteBefore(typeof(Target))]
            public class Before : StubCommand
            {
            }

            [Export]
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

            [Export]
            public class Target : StubCommand
            {
            }

            [DependsOn(typeof(Dependency)), ExecuteAfter(typeof(Target))]
            public class After : StubCommand
            {
            }

            [Export]
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

            [Shared, Export]
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

            [Export, DependsOn(typeof(Producer))]
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

            [Shared, ExecuteBefore(typeof(Consumer))]
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

            [Export]
            public class Consumer : StubCommand
            {
                [Import("BeforeExport")]
                public object Import { get; set; }
            }
        }

        public class Logging : EngineTest
        {
            [Fact]
            public void DirectsLoggingOfCommandsToOutputsImportedFromContainer()
            {
                var configuration = new ContainerConfiguration().WithParts(typeof(TestCommand), typeof(StubOutput));
                var engine = new Engine(configuration);

                engine.Execute<TestCommand>();

                Assert.Contains(TestCommand.TestRecord, StubOutput.WrittenRecords);
            }

            [Export]
            public class TestCommand : StubCommand
            {
                [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Record is immutable type")]
                public static readonly Record TestRecord = new Record("Test Value", RecordType.Error, Importance.High);

                public override void Execute()
                {
                    this.Log.Write(TestRecord);
                }
            }
        }
    }
}
