using System;
using System.Composition;
using System.Linq;
using Xunit;

namespace MefBuild
{
    public class SummaryAttributeTest
    {
        [Fact]
        public void ClassIsAttributeUsedToExtendMefMetadata()
        {
            Assert.True(typeof(SummaryAttribute).IsPublic);
            Assert.True(typeof(Attribute).IsAssignableFrom(typeof(SummaryAttribute)));
            Assert.True(typeof(SummaryAttribute).IsSealed);
            Assert.Contains(typeof(MetadataAttributeAttribute), typeof(SummaryAttribute).GetCustomAttributes(false).Select(a => a.GetType()));
        }

        [Fact]
        public void AttributeCanBeAppliedToCommandTypesToSpecifyTextDisplayedByHelpSystemForCommands()
        {
            object[] attributes = typeof(SummaryAttribute).GetCustomAttributes(false);
            var usage = attributes.OfType<AttributeUsageAttribute>().Single();
            Assert.Equal(AttributeTargets.Class, usage.ValidOn & AttributeTargets.Class);
        }

        [Fact]
        public void AttributeCanBeAppliedToPropertyAndParameterImportsToSpecifyTextDisplayedByHelpSystemForOptions()
        {
            object[] attributes = typeof(SummaryAttribute).GetCustomAttributes(false);
            var usage = attributes.OfType<AttributeUsageAttribute>().Single();
            Assert.Equal(AttributeTargets.Property, usage.ValidOn & AttributeTargets.Property);
            Assert.Equal(AttributeTargets.Parameter, usage.ValidOn & AttributeTargets.Parameter);
        }

        [Fact]
        public void ConstructorInitializesSummaryPropertyWithGivenValue()
        {
            var attribute = new SummaryAttribute("Test Value");
            Assert.Equal("Test Value", attribute.Summary);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new SummaryAttribute(null));
            Assert.Equal("summary", e.ParamName);
        }

        [Fact]
        public void SummaryIsReadOnlyBecauseItIsRequiredByConstructor()
        {
            Assert.Null(typeof(SummaryAttribute).GetProperty("Summary").SetMethod);
        }
    }
}
