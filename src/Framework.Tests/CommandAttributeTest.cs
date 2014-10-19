using System;
using System.Composition;
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
    }
}
