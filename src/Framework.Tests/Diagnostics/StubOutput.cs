using System;
using System.Collections.Generic;
using System.Composition;

namespace MefBuild.Diagnostics
{
    [Export, Export(typeof(Output)), Shared]
    internal class StubOutput : Output
    {
        private static readonly ICollection<Record> writtenRecords = new List<Record>();

        public static ICollection<Record> WrittenRecords
        {
            get { return writtenRecords; }
        }

        public StubOutput()
        {
            this.Verbosity = Verbosity.Diagnostic;
            this.OnWrite = record => writtenRecords.Add(record);
        }

        public Action<Record> OnWrite { get; set; }

        public override void Write(Record record)
        {
            this.OnWrite(record);
        }
    }
}
