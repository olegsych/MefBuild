using System;
using System.Composition;

namespace MefBuild.Hosting
{
    [Export, Export(typeof(Logger)), Shared]
    internal class StubLogger : Logger
    {
        public StubLogger()
        {
            this.OnWrite = (text, type, importance) => { };
        }

        public Action<string, EventType, EventImportance> OnWrite { get; set; }

        public override void Write(string message, EventType eventType, EventImportance importance)
        {
            this.OnWrite(message, eventType, importance);
        }
    }
}
