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
        public void CriticalWritesCriticalMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Critical Message";
            log.Critical(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(MessageType.Critical, log.LoggedMessageType);
        }

        [Fact]
        public void ErrorWritesErrorMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Error Message";
            log.Error(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(MessageType.Error, log.LoggedMessageType);
        }

        [Fact]
        public void WarningWritesWarningMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Warning Message";
            log.Warning(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(MessageType.Warning, log.LoggedMessageType);
        }

        [Fact]
        public void InformationWritesInformationMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Information Message";
            log.Information(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(MessageType.Information, log.LoggedMessageType);
        }

        [Fact]
        public void VerboseWritesVerboseMessageToLog()
        {
            var log = new TestableLog();

            string expectedMessage = "Verbose Message";
            log.Verbose(expectedMessage);

            Assert.Equal(expectedMessage, log.LoggedMessage);
            Assert.Equal(MessageType.Verbose, log.LoggedMessageType);
        }

        private class TestableLog : Log
        {
            public MessageType LoggedMessageType { get; set; }

            public string LoggedMessage { get; set; }

            protected override void Write(MessageType messageType, string message)
            {
                this.LoggedMessageType = messageType;
                this.LoggedMessage = message;
            }
        }
    }
}
