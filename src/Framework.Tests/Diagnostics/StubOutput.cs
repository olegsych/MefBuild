using System;
using System.Composition;

namespace MefBuild.Diagnostics
{
    [Export, Export(typeof(Output)), Shared]
    internal class StubOutput : Output
    {
        public StubOutput()
        {
            this.OnWrite = record => { };
        }

        public Action<Record> OnWrite { get; set; }

        public override void Write(Record record)
        {
            this.OnWrite(record);
        }
    }
}
