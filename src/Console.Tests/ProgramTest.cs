using System.Linq;
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
    }
}
