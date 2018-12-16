using System;
using System.IO;
using System.Threading.Tasks;
using NUglify;

namespace IEvangelist.DotNet.Miglifier.Core
{
    class MiglifyFile
    {
        readonly Func<string, UglifyResult> _uglify;

        internal MiglifyType Type { get; }

        internal string OriginalPath { get; }

        internal string MiglifiedPath { get; set; }

        internal Task<UglifyResult> ProcessAsync { get; }

        internal MiglifyFile(
            MiglifyType type, 
            string originalPath, 
            Func<string, UglifyResult> uglify)
        {
            Type = type;
            OriginalPath = originalPath;

            _uglify = uglify;

            ProcessAsync = GetProcessingTask();
        }

        async Task<UglifyResult> GetProcessingTask()
        {
            var file = await File.ReadAllTextAsync(OriginalPath);
            return _uglify(file);
        }

        public void Deconstruct(
            out string path,
            out Task<UglifyResult> resultTask)
        {
            path = OriginalPath;
            resultTask = ProcessAsync;
        }
    }
}