using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace MefBuild
{
    public class PrintProgramUsageTest
    {
        [Fact]
        public void ExecuteWritesUsageSectionToConsoleOutput()
        {
            string output = PrintProgramUsage();
            Assert.Contains("Usage:", output);
            Assert.Matches(new Regex(@"^\s+MefBuild <command> \[arguments]", RegexOptions.Multiline), output);
            Assert.Contains("For help about specific command, type:", output);
            Assert.Matches(new Regex(@"^\s+MefBuild help <command>", RegexOptions.Multiline), output);
        }

        private static string PrintProgramUsage()
        {
            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                var help = new PrintProgramUsage();
                help.Execute();
                return output.ToString();
            }
        }
    }
}
