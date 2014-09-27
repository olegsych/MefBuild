using System;
using System.Composition;

namespace MefBuild.Diagnostics
{
    [Export(typeof(Output))]
    internal class ConsoleOutput : Output
    {
        public override void Write(Record record)
        {
            if (object.ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = GetForegroundColor(record.RecordType, record.Importance);
            Console.WriteLine(record.Text);
            Console.ForegroundColor = oldColor;
        }

        private static ConsoleColor GetForegroundColor(RecordType eventType, Importance importance)
        {
            switch (eventType)
            {
                case RecordType.Error:   
                    return importance >= Importance.Normal ? ConsoleColor.Red : ConsoleColor.DarkRed;

                case RecordType.Warning: 
                    return importance >= Importance.Normal ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;

                case RecordType.Message: 
                    switch (importance)
                    {
                        case Importance.High: return ConsoleColor.White;
                        case Importance.Normal: return ConsoleColor.Gray;
                        case Importance.Low: return ConsoleColor.DarkGray;
                    }

                    break;

                case RecordType.Start:
                case RecordType.Stop:
                    return importance >= Importance.Normal ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
            }

            return default(ConsoleColor);
        }
    }
}
