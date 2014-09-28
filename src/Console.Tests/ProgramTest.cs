using System.Linq;
using System.Text;
using Xunit;

namespace MefBuild
{
    public static class ProgramTest
    {
        [Fact]
        public static void MainExecutesSpecifiedCommand()
        {
            TestCommand.ExecutedCommands.Clear();

            Program.Main(
                "-assembly:" + typeof(TestCommand).Assembly.Location,
                "-command:" + typeof(TestCommand).FullName);

            Assert.Equal(new[] { typeof(TestCommand).FullName }, TestCommand.ExecutedCommands.Select(c => c.GetType().FullName));
        }

        [Fact]
        public static void MainDirectsEngineOutputToConsole()
        {
            var output = new StringBuilder();
            using (new ConsoleOutputInterceptor(output))
            {
                Program.Main();
                Assert.Contains("Command \"" + typeof(Execute).FullName, output.ToString());
            }
        }

        [Fact]
        public static void MainDirectsEngineOutputToDebugger()
        {
            var output = new StringBuilder();
            using (new DebugOutputInterceptor(output))
            {
                Program.Main();
                Assert.Contains("Command \"" + typeof(Execute).FullName, output.ToString());
            }
        }
    }
}
