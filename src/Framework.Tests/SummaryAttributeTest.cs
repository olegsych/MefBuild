using System;
using System.Composition;
using System.Composition.Hosting;
using Xunit;

namespace MefBuild
{
    public class SummaryAttributeTest
    {
        private const string TestSummaryValue = "Test Summary Value";

        [Fact]
        public void ClassIsPublicSoThatUsersCanSpecifySummaryMetadataForTheirCommandsAndParameters()
        {
            Assert.True(typeof(SummaryAttribute).IsPublic);
        }

        [Fact]
        public void ConstructorInitializesSummaryPropertyWithSpecifiedValue()
        {
            var attribute = new SummaryAttribute(TestSummaryValue);
            Assert.Equal(TestSummaryValue, attribute.Summary);
        }

        [Fact]
        public void SummaryPropertyIsReadonlyBecauseItIsInitializedByConstructor()
        {
            Assert.Null(typeof(SummaryAttribute).GetProperty("Summary").SetMethod);
        }

        [Fact]
        public void AttributeCanBeAppliedToClassesToSupplyExportMetadata()
        {
            var container = new ContainerConfiguration().WithPart<ClassExportedWithSummary>().CreateContainer();

            var export = container.GetExport<Lazy<ClassExportedWithSummary, Metadata>>();

            Assert.Equal(TestSummaryValue, export.Metadata.Summary);
        }

        public class Metadata
        {
            public string Summary { get; set; }
        }

        [Export, Summary(TestSummaryValue)]
        public class ClassExportedWithSummary
        {
        }
    }
}
