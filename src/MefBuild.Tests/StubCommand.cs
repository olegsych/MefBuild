using System;

namespace MefBuild
{
    public class StubCommand : Command
    {
        public StubCommand() : base()
        {
            this.OnExecute = base.Execute;
        }

        public StubCommand(params Command[] dependsOn) : base(dependsOn)
        {
            this.OnExecute = base.Execute;
        }

        public Action OnExecute { get; set; }

        public override void Execute()
        {
            this.OnExecute();
        }
    }
}
