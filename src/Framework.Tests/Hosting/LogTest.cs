using System;
using Xunit;

namespace MefBuild.Hosting
{
    public class LogTest
    {
        [Fact]
        public void ClassIsPublicForLoggingFromUserCommands()
        {
            Assert.True(typeof(Log).IsPublic);
        }

        [Fact]
        public void ClassIsAbstractToAllowDifferentImplementations()
        {
            Assert.True(typeof(Log).IsAbstract);
        }

        [Fact]
        public void ErrorWritesErrorEventToLog()
        {
            var log = new TestableLog();

            string expectedText = "Error Message";
            log.Error(expectedText);

            Assert.Equal(expectedText, log.LoggedText);
            Assert.Equal(EventType.Error, log.LoggedEvent);
        }

        [Fact]
        public void WarningWritesWarningEventToLog()
        {
            var log = new TestableLog();

            string expectedText = "Warning Message";
            log.Warning(expectedText);

            Assert.Equal(expectedText, log.LoggedText);
            Assert.Equal(EventType.Warning, log.LoggedEvent);
        }

        [Fact]
        public void MessageWritesMessageEventToLog()
        {
            var log = new TestableLog();

            string expectedText = "Information Message";
            log.Message(expectedText);

            Assert.Equal(expectedText, log.LoggedText);
            Assert.Equal(EventType.Message, log.LoggedEvent);
        }

        private class TestableLog : Log
        {
            public EventType LoggedEvent { get; set; }

            public string LoggedText { get; set; }

            public override void Write(string text, EventType eventType, EventImportance importance)
            {
                this.LoggedEvent = eventType;
                this.LoggedText = text;
            }
        }
    }
}
