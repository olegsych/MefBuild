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

        [Import("OnExecute", AllowDefault = true)]
        public Action<ICommand> OnExecute { get; set; }

        public virtual void Execute()
        {
            this.OnExecute(this);
        }

        public override string ToString()
        {
            return this.GetHashCode().ToString("x");
        }
    }
}
