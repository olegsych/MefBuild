using System.Globalization;
using System.Linq;
using System.Reflection;
using MefBuild.Diagnostics;
using MefBuild.Properties;

namespace MefBuild
{
    internal static class EngineLogExtensions
    {
        internal static void CommandStarted(this Log log, Command command)
        {
            string text = string.Format(
                CultureInfo.CurrentCulture,
                Resources.CommandStartedMessage,
                command.GetType().FullName,
                command.GetType().GetTypeInfo().Assembly.Modules.First().FullyQualifiedName);
            log.Write(new Record(text, RecordType.Start, Importance.High));
        }

        internal static void CommandStopped(this Log log, Command command)
        {
            string text = string.Format(
                CultureInfo.CurrentCulture,
                Resources.CommandStoppedMessage,
                command.GetType().FullName);
            log.Write(new Record(text, RecordType.Stop, Importance.Normal));
        }
    }
}
