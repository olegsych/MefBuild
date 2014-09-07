using System;

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

        public Action<Command> OnExecute { get; set; }

        public override void Execute()
        {
            this.OnExecute(this);
        }
    }
}
