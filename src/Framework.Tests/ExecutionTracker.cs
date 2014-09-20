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
        private readonly List<Command> executedCommands = new List<Command>();

        public ExecutionTracker()
        {
            this.OnExecute = this.executedCommands.Add;
        }

        [Export("OnExecute")] 
        public Action<Command> OnExecute { get; set; }

        public void Verify(params Type[] expectedCommandTypes)
        {
            Assert.Equal(expectedCommandTypes, this.executedCommands.Select(c => c.GetType()));
        }
    }
}
