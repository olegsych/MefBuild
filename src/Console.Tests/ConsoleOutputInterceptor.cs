using System;
using System.IO;
using System.Text;

namespace MefBuild
{
    internal sealed class ConsoleOutputInterceptor : IDisposable
    {
        private readonly TextWriter oldOut;

        public ConsoleOutputInterceptor(StringBuilder output)
        {
            this.oldOut = Console.Out;
            Console.SetOut(new StringWriter(output));
        }

        public void Dispose()
        {
            Console.SetOut(this.oldOut);
        }
    }
}
