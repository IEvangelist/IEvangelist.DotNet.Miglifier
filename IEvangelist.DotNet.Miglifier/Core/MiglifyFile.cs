using System;
using System.IO;
using System.Threading.Tasks;
using NUglify;

namespace IEvangelist.DotNet.Miglifier.Core
{
    internal class MiglifyFile
    {
        internal TargetType Type { get; }

        internal string OriginalPath { get; }

        internal string MiglifiedPath { get; set; }

        internal Task<UglifyResult> ProcessAsync { get; }

        internal MiglifyFile(TargetType type, string originalPath)
        {
            Type = type;
            OriginalPath = originalPath;
            ProcessAsync = GetProcessingTask();
        }

        async Task<UglifyResult> GetProcessingTask()
        {
            var file = await File.ReadAllTextAsync(OriginalPath);
            switch (Type)
            {
                case TargetType.Css:
                    return Uglify.Css(file);
                case TargetType.Html:
                    return Uglify.Html(file);
                case TargetType.Js:
                    return Uglify.Js(file);

                default:
                    throw new ArgumentException(nameof(Type));
            }
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