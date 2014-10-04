using System.Globalization;
using System.Linq;
using System.Reflection;
using MefBuild.Diagnostics;

namespace MefBuild
{
    internal static class EngineLogExtensions
    {
        internal static void CommandStarted(this Log log, Command command)
        {
            string text = string.Format(
                CultureInfo.CurrentCulture,
                Messages.CommandStartedMessage,
                command.GetType().FullName,
                command.GetType().GetTypeInfo().Assembly.Modules.First().FullyQualifiedName);
            log.Write(new Record(text, RecordType.Start, Importance.High));
        }

        internal static void CommandStopped(this Log log, Command command)
        {
            string text = string.Format(
                CultureInfo.CurrentCulture,
                Messages.CommandStoppedMessage,
                command.GetType().FullName);
            log.Write(new Record(text, RecordType.Stop, Importance.Normal));
        }
    }
}
