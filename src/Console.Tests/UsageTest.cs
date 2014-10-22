using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace MefBuild
{
    public class UsageTest
    {
        [Fact]
        public void UsagePrintsProgramUsage()
        {
            string programUsage = ExecuteCommands(typeof(PrintProgramUsage));
            string allUsage = ExecuteCommands(typeof(Usage));

            Assert.Contains(programUsage, allUsage);
        }

        [Fact]
        public void UsagePrintsNamedProgramCommands()
        {
            string namedProgramCommands = ExecuteCommands(typeof(GetProgramCommandTypes), typeof(PrintNamedCommands));
            string allUsage = ExecuteCommands(typeof(Usage));

            Assert.NotEmpty(namedProgramCommands);
            Assert.Contains(namedProgramCommands, allUsage);
        }

        private static string ExecuteCommands(params Type[] commandTypes)
        {
            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                var configuration = new ContainerConfiguration().WithParts(GetProgramCommands());
                var engine = new Engine(configuration);
                foreach (Type commandType in commandTypes)
                {
                    engine.Execute(commandType);
                }

                return output.ToString();
            }
        }

        private static IEnumerable<Type> GetProgramCommands()
        {
            return typeof(Program).Assembly.DefinedTypes
                .Where(t => t.GetCustomAttributes<ExportAttribute>().Any() && typeof(Command).IsAssignableFrom(t));
        }
    }
}
