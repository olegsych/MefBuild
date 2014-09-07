using System;
using System.Composition;

namespace MefBuild
{
    public class StubCommand : Command
    {
        public StubCommand() : base()
        {
            this.OnExecute = @this => base.Execute();
        }

        public StubCommand(params Command[] dependsOn) : base(dependsOn)
        {
            this.OnExecute = @this => base.Execute();
        }

        [Import("OnExecute")]
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
