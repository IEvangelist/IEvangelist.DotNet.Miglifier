using System.Collections.Generic;

namespace IEvangelist.DotNet.Miglifier.Core
{
    class MiglifyResult
    {
        internal int ExitCode { get; }

        internal IEnumerable<MiglifyFile> Files { get; }

        internal MiglifyResult(
            int exitCode,
            IEnumerable<MiglifyFile> files = null)
        {
            ExitCode = exitCode;
            Files = files;
        }
    }
}