using System;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MefBuild
{
    public class CommandAttributeTest
    {
        [Fact]
        public void CommandAttributeExportsClassWithCommandContractTypeAndAdditionalMetadata()
        {
            Assert.True(typeof(CommandAttribute).IsPublic);
            Assert.True(typeof(CommandAttribute).IsSealed);
            Assert.True(typeof(ExportAttribute).IsAssignableFrom(typeof(CommandAttribute)));
            Assert.NotNull(typeof(CommandAttribute).GetCustomAttribute<MetadataAttributeAttribute>());
            Assert.Equal(AttributeTargets.Class, typeof(CommandAttribute).GetCustomAttribute<AttributeUsageAttribute>().ValidOn);
        }

        [Fact]
        public void CommandTypeProvidesCommandTypeMetadata()
        {
            var context = new ContainerConfiguration().WithPart<TestCommand>().CreateContainer();
            var export = context.GetExport<ExportFactory<Command, CommandMetadata>>();
            Assert.Equal(typeof(TestCommand).GetCustomAttribute<CommandAttribute>().CommandType, export.Metadata.CommandType);
        }

        [Fact]
        public void DependsOnProvidesDependsOnCommandMetadata()
        {
            var context = new ContainerConfiguration().WithPart<TestCommand>().CreateContainer();
            var export = context.GetExport<ExportFactory<Command, CommandMetadata>>();
            Assert.Equal(typeof(TestCommand).GetCustomAttribute<CommandAttribute>().DependsOn, export.Metadata.DependsOn);
        }

        [Fact]
        public void ExecuteBeforeProvidesExecuteBeforeCommandMetadata()
        {
            var context = new ContainerConfiguration().WithPart<TestCommand>().CreateContainer();
            var export = context.GetExport<ExportFactory<Command, CommandMetadata>>();
            Assert.Equal(typeof(TestCommand).GetCustomAttribute<CommandAttribute>().ExecuteBefore, export.Metadata.ExecuteBefore);
        }

        [Fact]
        public void ExecuteAfterProvidesExecuteAfterCommandMetadata()
        {
            var context = new ContainerConfiguration().WithPart<TestCommand>().CreateContainer();
            var export = context.GetExport<ExportFactory<Command, CommandMetadata>>();
            Assert.Equal(typeof(TestCommand).GetCustomAttribute<CommandAttribute>().ExecuteAfter, export.Metadata.ExecuteAfter);
        }

        [Fact]
        public void SummaryProvidesSummaryCommandMetadata()
        {
            var context = new ContainerConfiguration().WithPart<TestCommand>().CreateContainer();
            var export = context.GetExport<ExportFactory<Command, CommandMetadata>>();
            Assert.Equal(typeof(TestCommand).GetCustomAttribute<CommandAttribute>().Summary, export.Metadata.Summary);
        }

        [Command(
            CommandType = typeof(TestCommand),
            DependsOn = new[] { typeof(StubCommand) },
            ExecuteBefore = new[] { typeof(StubCommand) },
            ExecuteAfter = new[] { typeof(StubCommand) },
            Summary = "Test Summary")]
        public class TestCommand : Command
        {
        }
    }
}
