using IEvangelist.DotNet.Miglifier.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IEvangelist.DotNet.Miglifier.Config;
using IEvangelist.DotNet.Miglifier.Extensions;
using Xunit;

namespace IEvangelist.DotNet.MiglifierTests
{
    public class MinifierAndUglifierTests
    {
        [Fact]
        public async Task ThrowsArgumentExceptionWithInvalidWwwRootPath()
            => await Assert.ThrowsAsync<ArgumentException>(
                () => MinifierAndUglifier.MiglifyAsync("/Pickles/", null));

        [Fact]
        public async Task MinifiesAndUglifiesAllContentsOfWwwRoot()
        {
            var result = await MinifierAndUglifier.MiglifyAsync("wwwroot", null);
            
            AssertExitCodeAndFileExist(result);
            AssertFileSizeAndDates(result);

            Assert.Contains(result.Files, _ => _.Type == MiglifyType.Css);
            Assert.Contains(result.Files, _ => _.Type == MiglifyType.Js);
            Assert.Contains(result.Files, _ => _.Type == MiglifyType.Html);
        }

        [
            Theory,
            InlineData("css", MiglifyType.Css),
            InlineData("js", MiglifyType.Js),
            InlineData("html", MiglifyType.Html)
        ]
        public async Task MinifiesAndUglifiesAllContentsOf(string path, MiglifyType type) 
            => AssertFileSizeDateAndTypes(type, await MinifierAndUglifier.MiglifyAsync(path, null));

        [Fact]
        public async Task MiglifiesAndOutputsInDist()
        {
            var settings = new MiglifySettings();
            settings.Globs[MiglifyType.Js].Output = "dist";

            const string miglifyJson = "miglify.json";
            await File.WriteAllTextAsync(miglifyJson, settings.ToJson());

            var result = await MinifierAndUglifier.MiglifyAsync("js", miglifyJson);
            AssertFileSizeDateAndTypes(MiglifyType.Js, result);
        }

        static void AssertExitCodeAndFileExist(MiglifyResult result)
        {
            Assert.True(result.ExitCode == 0, "Expected successful execution.");
            Assert.True(result.Files.Any(), "Expected results to have output.");
        }

        static void AssertFileSizeDateAndTypes(MiglifyType type, MiglifyResult result)
        {
            AssertExitCodeAndFileExist(result);
            AssertFileSizeAndDates(result);

            Assert.True(result.Files.All(t => t.Type == type),
                        $"Expected {type} output.");
        }

        static void AssertFileSizeAndDates(MiglifyResult result)
        {
            foreach (var file in result.Files)
            {
                var originalFile = new FileInfo(file.OriginalPath);
                var minifiedFile = new FileInfo(file.MiglifiedPath);
                Assert.True(originalFile.Length > minifiedFile.Length,
                            $"Expected minification to make {file.Type} file smaller.");
                Assert.True(minifiedFile.LastWriteTimeUtc > originalFile.LastWriteTimeUtc,
                            $"Expected {file.Type} file to have been updated.");
            }
        }
    }
}