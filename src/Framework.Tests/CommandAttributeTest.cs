using System;
using System.Composition;
using System.Composition.Hosting;
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

        [Command(
            CommandType = typeof(TestCommand),
            ExecuteBefore = new[] { typeof(StubCommand) },
            ExecuteAfter = new[] { typeof(StubCommand) })]
        public class TestCommand : Command
        {
        }
    }
}
