﻿using System;
using System.Composition;
using MefBuild.Hosting;

namespace MefBuild
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

        private static ConsoleColor GetForegroundColor(EventType eventType, EventImportance importance)
        {
            switch (eventType)
            {
                case EventType.Error:   
                    return importance >= EventImportance.Normal ? ConsoleColor.Red : ConsoleColor.DarkRed;

                case EventType.Warning: 
                    return importance >= EventImportance.Normal ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;

                case EventType.Message: 
                    switch (importance)
                    {
                        case EventImportance.High: return ConsoleColor.White;
                        case EventImportance.Normal: return ConsoleColor.Gray;
                        case EventImportance.Low: return ConsoleColor.DarkGray;
                    }

                    break;

                case EventType.Start:
                case EventType.Stop:
                    return importance >= EventImportance.Normal ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
            }

            return default(ConsoleColor);
        }
    }
}
