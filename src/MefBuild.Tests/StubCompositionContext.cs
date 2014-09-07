using System;
using System.Composition;
using System.Composition.Hosting.Core;

namespace MefBuild
{
    public class StubCompositionContext : CompositionContext
    {
        public StubCompositionContext()
        {
            this.OnTryGetExport = (CompositionContract contract, out object export) =>
            {
                export = null;
                return false;
            };
        }

        public delegate bool TryGetExportDelegate(CompositionContract contract, out object export);

        public TryGetExportDelegate OnTryGetExport { get; set; }

        public override bool TryGetExport(CompositionContract contract, out object export)
        {
            return this.OnTryGetExport(contract, out export);
        }
    }
}
