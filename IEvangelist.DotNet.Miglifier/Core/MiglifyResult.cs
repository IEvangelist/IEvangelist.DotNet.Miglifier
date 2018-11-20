using System.Collections.Generic;

namespace IEvangelist.DotNet.Miglifier.Core
{
    class MiglifyResult
    {
        internal int ExitCode { get; }

        internal IEnumerable<(string InputPath, string OutputPath, TargetType Type)> Output { get; }

        internal MiglifyResult(
            int exitCode,
            IEnumerable<(string InputPath, string OutputPath, TargetType Type)> output = null)
        {
            ExitCode = exitCode;
            Output = output;
        }
    }
}