using System;
using System.Collections.Generic;
using System.Composition;

namespace MefBuild
{
    public class StubCommand : Command
    {
        private static readonly ICollection<Command> executedCommands = new List<Command>();

        public static ICollection<Command> ExecutedCommands
        {
            get { return executedCommands; }
        }

        public StubCommand()
        {
            this.OnExecute = @this => { ExecutedCommands.Add(@this); };
        }

        [Import("OnExecute", AllowDefault = true)]
        public Action<Command> OnExecute { get; set; }

        public override void Execute()
        {
            this.OnExecute(this);
        }

        public override string ToString()
        {
            return this.GetHashCode().ToString("x");
        }
    }
}
