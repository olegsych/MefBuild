using System;
using System.Composition;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class OptionAttributeTest
    {
        [Fact]
        public void ClassIsMetadataAttributeThatCanBeUsedWithPropertiesAndParametersOfUserCommands()
        {
            Assert.True(typeof(OptionAttribute).IsPublic);
            Assert.True(typeof(Attribute).IsAssignableFrom(typeof(OptionAttribute)));
            Assert.True(typeof(OptionAttribute).IsSealed);

            object[] attributes = typeof(OptionAttribute).GetCustomAttributes(false);
            Assert.Contains(typeof(MetadataAttributeAttribute), attributes.Select(a => a.GetType()));

            var attributeUsage = attributes.OfType<AttributeUsageAttribute>().Single();
            Assert.Equal(AttributeTargets.Property | AttributeTargets.Parameter, attributeUsage.ValidOn);
        }

        [Fact]
        public void DescriptionIsEmptyStringByDefaultToPreventUsageErrors()
        {
            var attribute = new OptionAttribute();
            string description = attribute.Description;
            Assert.Empty(description);
        }

        [Fact]
        public void DescriptionCanBeSetByUserToSpecifyOptionDescription()
        {
            var attribute = new OptionAttribute();
            attribute.Description = "Test Value";
            Assert.Equal("Test Value", attribute.Description);
        }

        [Fact]
        public void DescriptionThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var attribute = new OptionAttribute();
            var e = Assert.Throws<ArgumentNullException>(() => attribute.Description = null);
            Assert.Equal("value", e.ParamName);
        }
    }
}
