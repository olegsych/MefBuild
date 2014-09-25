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
        public void ErrorWritesErrorMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Error Message";
            log.Error(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(EventType.Error, log.LoggedMessageType);
        }

        [Fact]
        public void WarningWritesWarningMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Warning Message";
            log.Warning(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(EventType.Warning, log.LoggedMessageType);
        }

        [Fact]
        public void InformationWritesInformationMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Information Message";
            log.Information(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(EventType.Information, log.LoggedMessageType);
        }

        private class TestableLog : Log
        {
            public EventType LoggedMessageType { get; set; }

            public string LoggedMessage { get; set; }

            protected override void Write(EventType messageType, string message)
            {
                this.LoggedMessageType = messageType;
                this.LoggedMessage = message;
            }
        }
    }
}
