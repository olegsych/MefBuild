using System;
using System.IO;
using System.Text;

namespace MefBuild
{
    internal sealed class ConsoleOutputInterceptor : IDisposable
    {
        private readonly StringBuilder output;
        private readonly TextWriter oldOut;

        public ConsoleOutputInterceptor() : this(new StringBuilder())
        {
        }

        public ConsoleOutputInterceptor(StringBuilder output)
        {
            this.oldOut = Console.Out;
            this.output = output;
            Console.SetOut(new StringWriter(output));
        }

        public string Output
        {
            get { return this.output.ToString(); }
        }

        public void Dispose()
        {
            Console.SetOut(this.oldOut);
        }
    }
}
