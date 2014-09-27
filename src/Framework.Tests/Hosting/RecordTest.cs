using System;
using Xunit;

namespace MefBuild.Hosting
{
    public class RecordTest
    {
        [Fact]
        public void ClassIsPublicForOutputImplementations()
        {
            Assert.True(typeof(Record).IsPublic);
        }

        [Fact]
        public void ClassIsSealedBecauseItEncapsulatesAllVariationsOfLogRecords()
        {
            Assert.True(typeof(Record).IsSealed);
        }

        [Fact]
        public void ConstructorInitializesAllProperties()
        {
            var record = new Record("Test Text", EventType.Error, EventImportance.High);
            Assert.Equal("Test Text", record.Text);
            Assert.Equal(EventType.Error, record.RecordType);
            Assert.Equal(EventImportance.High, record.Importance);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenTextIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new Record(null, default(EventType), default(EventImportance)));
            Assert.Equal("text", e.ParamName);
        }

        [Fact]
        public void ConstructorAllowsEmptyTextSoThatBlankRecordsCanBeUsedToMakeOutputMoreReadable()
        {
            var record = new Record(string.Empty, default(EventType), default(EventImportance));
            Assert.Equal(string.Empty, record.Text);
        }

        [Fact]
        public void PropertiesAreReadOnlyBecauseRecordInstanceIsImmutable()
        {
            Assert.Null(typeof(Record).GetProperty("Text").SetMethod);
            Assert.Null(typeof(Record).GetProperty("Type").SetMethod);
            Assert.Null(typeof(Record).GetProperty("Importance").SetMethod);
        }
    }
}
