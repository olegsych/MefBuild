using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Xunit;

namespace MefBuild
{
    internal static class CSharpCompiler
    {
        public static Assembly CompileInMemory(string code, params Assembly[] references)
        {
            string[] referenceFileNames = references.Select(a => a.Location).ToArray();

            var compilerParameters = new CompilerParameters(referenceFileNames);
            compilerParameters.GenerateInMemory = true;

            var codeProvider = new CSharpCodeProvider();
            CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, code);

            if (compilerResults.Errors.HasErrors)
            {
                throw new Exception(string.Join(Environment.NewLine, compilerResults.Errors.Cast<CompilerError>()));
            }

            return compilerResults.CompiledAssembly;
        }
    }
}
