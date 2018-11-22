using IEvangelist.DotNet.Miglifier.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IEvangelist.DotNet.MiglifierTests
{
    public class MinifierAndUglifierTests
    {
        [Fact]
        public async Task ThrowsArgumentExceptionWithInvalidWwwRootPath()
            => await Assert.ThrowsAsync<ArgumentException>(
                () => MinifierAndUglifier.MiglifyAsync("/Pickles/"));

        [Fact]
        public async Task MinifiesAndUglifiesAllContentsOfWwwRoot()
        {
            var result = await MinifierAndUglifier.MiglifyAsync("wwwroot");
            Assert.True(result.ExitCode == 0, "Expected successful execution.");
            Assert.True(result.Files.Any(), "Expected results to have output.");

            foreach (var file in result.Files)
            {
                var originalFile = new FileInfo(file.OriginalPath);
                var minifiedFile = new FileInfo(file.MiglifiedPath);

                Assert.True(originalFile.Length > minifiedFile.Length,
                            $"Expected minification to make {file.Type} file smaller.");
                Assert.True(minifiedFile.LastWriteTimeUtc > originalFile.LastWriteTimeUtc,
                            $"Expected {file.Type} file to have been updated.");
            }

            Assert.Contains(result.Files, _ => _.Type == TargetType.Css);
            Assert.Contains(result.Files, _ => _.Type == TargetType.Js);
            Assert.Contains(result.Files, _ => _.Type == TargetType.Html);
        }

        [
            Theory,
            InlineData("css", TargetType.Css),
            InlineData("js", TargetType.Js),
            InlineData("html", TargetType.Html)
        ]
        public async Task MinifiesAndUglifiesAllContentsOf(string path, TargetType type)
        {
            var result = await MinifierAndUglifier.MiglifyAsync(path);
            Assert.True(result.ExitCode == 0, "Expected successful execution.");
            Assert.True(result.Files.Any(), "Expected results to have output.");

            foreach (var file in result.Files)
            {
                var originalFile = new FileInfo(file.OriginalPath);
                var minifiedFile = new FileInfo(file.MiglifiedPath);

                Assert.True(originalFile.Length > minifiedFile.Length,
                            $"Expected minification to make {file.Type} file smaller.");
                Assert.True(minifiedFile.LastWriteTimeUtc > originalFile.LastWriteTimeUtc,
                            $"Expected {file.Type} file to have been updated.");
            }

            Assert.True(result.Files.All(t => t.Type == type),
                        $"Expected {type} output.");
        }
    }
}