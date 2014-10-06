using System.Collections.Generic;
using System.Composition;

namespace MefBuild
{
    [Command]
    public class TestCommand : Command
    {
        private static ICollection<Command> executedCommands = new List<Command>();

        public static ICollection<Command> ExecutedCommands
        {
            get { return executedCommands; }
        }

        public override void Execute()
        {
            executedCommands.Add(this);
        }
    }
}
