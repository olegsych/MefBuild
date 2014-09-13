using System;
using System.Composition;

namespace MefBuild
{
    public class StubCommand : Command
    {
        public StubCommand()
        {
            this.OnExecute = @this => { };
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
