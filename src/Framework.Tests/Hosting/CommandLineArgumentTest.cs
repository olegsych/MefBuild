using System;
using Xunit;

namespace MefBuild.Hosting
{
    public class CommandLineArgumentTest
    {
        [Fact]
        public void ClassIsInternalAndNotMeantToBeUsedByUsers()
        {
            Assert.False(typeof(CommandLineArgument).IsPublic);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionToPreventUsageErrors()
        {
            string argument = null;
            var e = Assert.Throws<ArgumentNullException>(() => new CommandLineArgument(argument));
            Assert.Equal("argument", e.ParamName);
        }

        [Fact]
        public void ConstructorSetsNameFromSimpleArgument()
        {
            var arg = new CommandLineArgument("TestName");
            Assert.Equal("TestName", arg.Name);
        }

        [Fact]
        public void ConstructorSetsNameFromArgumentWithLeadingDash()
        {
            var arg = new CommandLineArgument("/TestName");
            Assert.Equal("TestName", arg.Name);
        }

        [Fact]
        public void ConstructorSetsNameAndValueFromArgumentWithEqualsSign()
        {
            var arg = new CommandLineArgument("/TestName=TestValue");
            Assert.Equal("TestName", arg.Name);
            Assert.Equal("TestValue", arg.Value);
        }

        [Fact]
        public void ConstructorTreatsAdditionalEqualsSignsAsPartOfValue()
        {
            var arg = new CommandLineArgument("/TestName=Test=Value");
            Assert.Equal("Test=Value", arg.Value);
        }

        [Fact]
        public void ConstructorInitializesValueWithEmptyStringToPreventNullReferenceExceptions()
        {
            var arg = new CommandLineArgument("ArgumentWithoutValue");
            Assert.Equal(string.Empty, arg.Value);
        }

        [Fact]
        public void ConstructorInitializesOriginalWithOriginalArgumentString()
        {
            var arg = new CommandLineArgument("/TestName=TestValue");
            Assert.Equal("/TestName=TestValue", arg.Original);
        }

        [Fact]
        public void NameIsReadOnlyBecauseClassIsImmutable()
        {
            Assert.Null(typeof(CommandLineArgument).GetProperty("Name").SetMethod);
        }

        [Fact]
        public void OriginalIsReadOnlyBecauseClassIsImmutable()
        {
            Assert.Null(typeof(CommandLineArgument).GetProperty("Original").SetMethod);
        }

        [Fact]
        public void ValueIsReadOnlyBecauseClassIsImmutable()
        {
            Assert.Null(typeof(CommandLineArgument).GetProperty("Value").SetMethod);
        }
    }
}
