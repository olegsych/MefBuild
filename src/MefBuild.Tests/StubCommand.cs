using System;
using System.Composition;

namespace MefBuild
{
    public class StubCommand : ICommand
    {
        public StubCommand()
        {
            this.OnExecute = @this => { };
        }

        [Import("OnExecute")]
        public Action<ICommand> OnExecute { get; set; }

        public void Execute()
        {
            this.OnExecute(this);
        }

        public override string ToString()
        {
            return this.GetHashCode().ToString("x");
        }
    }
}
