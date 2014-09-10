using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Xunit;

namespace MefBuild
{
    [Export, Shared]
    public class ExecutionTracker
    {
        private readonly List<ICommand> executedCommands = new List<ICommand>();

        public ExecutionTracker()
        {
            this.OnExecute = this.executedCommands.Add;
        }

        [Export("OnExecute")] 
        public Action<ICommand> OnExecute { get; set; }

        public void Verify(params Type[] expectedCommandTypes)
        {
            Assert.Equal(expectedCommandTypes, this.executedCommands.Select(c => c.GetType()));
        }
    }
}
