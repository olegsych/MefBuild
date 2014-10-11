using System;
using Xunit;

namespace MefBuild.Diagnostics
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
            var record = new Record("Test Text", RecordType.Error, Importance.High);
            Assert.Equal("Test Text", record.Text);
            Assert.Equal(RecordType.Error, record.RecordType);
            Assert.Equal(Importance.High, record.Importance);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenTextIsNullToPreventUsageErrors()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new Record(null, default(RecordType), default(Importance)));
            Assert.Equal("text", e.ParamName);
        }

        [Fact]
        public void ConstructorAllowsEmptyTextSoThatBlankRecordsCanBeUsedToMakeOutputMoreReadable()
        {
            var record = new Record(string.Empty, default(RecordType), default(Importance));
            Assert.Equal(string.Empty, record.Text);
        }

        [Fact]
        public void PropertiesAreReadOnlyBecauseRecordInstanceIsImmutable()
        {
            Assert.Null(typeof(Record).GetProperty("Text").SetMethod);
            Assert.Null(typeof(Record).GetProperty("RecordType").SetMethod);
            Assert.Null(typeof(Record).GetProperty("Importance").SetMethod);
        }

        [Fact]
        public void EqualsReturnsFalseWhenGivenInstanceIsNull()
        {
            var source = new Record(string.Empty, default(RecordType), default(Importance));
            Assert.False(source.Equals(null));
        }

        [Fact]
        public void EqualsReturnsFalseWhenOtherObjectIsNotRecord()
        {
            var source = new Record(string.Empty, default(RecordType), default(Importance));
            var target = new object();
            Assert.False(source.Equals(target));
        }

        [Fact]
        public void RecordsAreEqualWhenAllPropertiesAreEqual()
        {
            var source = new Record(string.Empty, default(RecordType), default(Importance));
            var target = new Record(string.Empty, default(RecordType), default(Importance));
            Assert.True(source.Equals(target));
            Assert.Equal(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void RecordsAreNotEqualWhenTextValuesAreDifferent()
        {
            var source = new Record("Source Text", default(RecordType), default(Importance));
            var target = new Record("Target Text", default(RecordType), default(Importance));
            Assert.False(source.Equals(target));
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void RecordsAreNotEqualWhenRecordTypesAreDifferent()
        {
            var source = new Record(string.Empty, RecordType.Error,   default(Importance));
            var target = new Record(string.Empty, RecordType.Warning, default(Importance));
            Assert.False(source.Equals(target));
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void RecordsAreNotEqualWhenImportanceValuesAreDifferent()
        {
            var source = new Record(string.Empty, default(RecordType), Importance.Low);
            var target = new Record(string.Empty, default(RecordType), Importance.High);
            Assert.False(source.Equals(target));
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
        }
    }
}
