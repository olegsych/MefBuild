using System;
using System.Composition;

namespace MefBuild.Hosting
{
    [Export, Export(typeof(Output)), Shared]
    internal class StubOutput : Output
    {
        public StubOutput()
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
