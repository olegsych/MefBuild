using System.Globalization;
using System.Linq;
using System.Reflection;
using MefBuild.Diagnostics;
using MefBuild.Execution;

namespace MefBuild
{
    internal static class EngineLogExtensions
    {
        internal static void CommandStarted(this Log log, ExecutionStep step)
        {
            TypeInfo commandType = step.Command.Metadata.CommandType.GetTypeInfo();
            string commandName = commandType.FullName;
            string commandAssembly = commandType.Assembly.Modules.First().FullyQualifiedName;
            string dependencyExplanation = GetDependencyExplanation(step);
            string text = string.Format(
                CultureInfo.CurrentCulture,
                Messages.CommandStartedMessage,
                commandName,
                commandAssembly,
                dependencyExplanation);
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

        private static string GetDependencyExplanation(ExecutionStep step)
        {
            switch (step.DependencyType)
            {
                case DependencyType.DependsOn:
                    return string.Format(
                        CultureInfo.CurrentCulture, 
                        Messages.DependsOnDependencyExplanation, 
                        step.Dependency.Metadata.CommandType.FullName);

                case DependencyType.ExecuteBefore: 
                    return string.Format(
                        CultureInfo.CurrentCulture, 
                        Messages.ExecuteBeforeDependencyExplanation,
                        step.Dependency.Metadata.CommandType.FullName);

                case DependencyType.ExecuteAfter:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        Messages.ExecuteAfterDependencyExplanation,
                        step.Dependency.Metadata.CommandType.FullName);

                default:
                    return string.Empty;
            }
        }
    }
}
