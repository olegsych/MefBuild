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
        public void ClassImplementsIEquatableToSimplifyTestingOfLoggingFunctionality()
        {
            Assert.True(typeof(IEquatable<Record>).IsAssignableFrom(typeof(Record)));
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
            Assert.Null(typeof(Record).GetProperty("RecordType").SetMethod);
            Assert.Null(typeof(Record).GetProperty("Importance").SetMethod);
        }

        [Fact]
        public void EqualsOfRecordReturnsFalseWhenGivenInstanceIsNull()
        {
            var source = new Record(string.Empty, default(EventType), default(EventImportance));
            Assert.False(source.Equals(null));
        }

        [Fact]
        public void RecordsAreEqualWhenTheyAreBothNull()
        {
            Record source = null;
            Record target = null;
            Assert.True(source == target);
            Assert.False(source != target);
        }

        [Fact]
        public void RecordsAreNotEqualWhenOneOfThemIsNull()
        {
            Record realRecord = new Record(string.Empty, default(EventType), default(EventImportance));
            Record nullRecord = null;

            Assert.False(object.Equals(realRecord, null));
            Assert.False(realRecord == nullRecord);
            Assert.True(realRecord != nullRecord);

            Assert.False(object.Equals(nullRecord, realRecord));
            Assert.False(nullRecord == realRecord);
            Assert.True(nullRecord != realRecord);
        }

        [Fact]
        public void RecordsAreEqualWhenAllPropertiesAreEqual()
        {
            var source = new Record(string.Empty, default(EventType), default(EventImportance));
            var target = new Record(string.Empty, default(EventType), default(EventImportance));
            Assert.True(object.Equals(source, target));
            Assert.True(source.Equals(target));
            Assert.True(source == target);
            Assert.False(source != target);
            Assert.Equal(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void RecordsAreNotEqualWhenTextValuesAreDifferent()
        {
            var source = new Record("Source Text", default(EventType), default(EventImportance));
            var target = new Record("Target Text", default(EventType), default(EventImportance));
            Assert.False(object.Equals(source, target));
            Assert.False(source.Equals(target));
            Assert.False(source == target);
            Assert.True(source != target);
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void RecordsAreNotEqualWhenRecordTypesAreDifferent()
        {
            var source = new Record(string.Empty, EventType.Error,   default(EventImportance));
            var target = new Record(string.Empty, EventType.Warning, default(EventImportance));
            Assert.False(object.Equals(source, target));
            Assert.False(source.Equals(target));
            Assert.False(source == target);
            Assert.True(source != target);
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void RecordsAreNotEqualWhenImportanceValuesAreDifferent()
        {
            var source = new Record(string.Empty, default(EventType), EventImportance.Low);
            var target = new Record(string.Empty, default(EventType), EventImportance.High);
            Assert.False(object.Equals(source, target));
            Assert.False(source.Equals(target));
            Assert.False(source == target);
            Assert.True(source != target);
            Assert.NotEqual(source.GetHashCode(), target.GetHashCode());
        }

        [Fact]
        public void ObjectsAreNotEqualWhenOtherObjectIsNotRecord()
        {
            var source = new Record(string.Empty, default(EventType), default(EventImportance));
            var target = new object();
            Assert.False(object.Equals(source, target));           
            Assert.False(source.Equals(target));
        }
    }
}
