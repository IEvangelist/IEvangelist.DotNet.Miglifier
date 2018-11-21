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
            var startDateTime = DateTime.UtcNow;
            var result = await MinifierAndUglifier.MiglifyAsync("wwwroot");
            Assert.True(result.ExitCode == 0, "Expected successful execution.");
            Assert.True(result.Output.Any(), "Expected results to have output.");

            foreach (var (input, output, type) in result.Output)
            {
                var originalSize = new FileInfo(input).Length;
                var minifiedFile = new FileInfo(output);

                Assert.True(originalSize > minifiedFile.Length,
                            $"Expected minification to make {type} file smaller.");
                Assert.True(minifiedFile.LastWriteTimeUtc > startDateTime,
                            $"Expected {type} file to have been updated.");
            }

            Assert.Contains(result.Output, _ => _.Type == TargetType.Css);
            Assert.Contains(result.Output, _ => _.Type == TargetType.Js);
            Assert.Contains(result.Output, _ => _.Type == TargetType.Html);
        }

        [
            Theory,
            InlineData("css", TargetType.Css),
            InlineData("js", TargetType.Js),
            InlineData("html", TargetType.Html)
        ]
        public async Task MinifiesAndUglifiesAllContentsOf(string path, TargetType type)
        {
            var startDateTime = DateTime.UtcNow;
            var result = await MinifierAndUglifier.MiglifyAsync(path);
            Assert.True(result.ExitCode == 0, "Expected successful execution.");
            Assert.True(result.Output.Any(), "Expected results to have output.");

            foreach (var (input, output, _) in result.Output)
            {
                var originalSize = new FileInfo(input).Length;
                var minifiedFile = new FileInfo(output);

                Assert.True(originalSize > minifiedFile.Length,
                            $"Expected minification to make {type} file smaller.");
                Assert.True(minifiedFile.LastWriteTimeUtc > startDateTime,
                            $"Expected {type} file to have been updated.");
            }

            Assert.True(result.Output.All(t => t.Type == type),
                        $"Expected {type} output.");
        }
    }
}