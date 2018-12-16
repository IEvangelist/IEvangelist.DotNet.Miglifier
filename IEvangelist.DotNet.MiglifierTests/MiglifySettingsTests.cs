using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IEvangelist.DotNet.Miglifier.Config;
using IEvangelist.DotNet.Miglifier.Core;
using IEvangelist.DotNet.Miglifier.Extensions;
using Xunit;

namespace IEvangelist.DotNet.MiglifierTests
{
    public class MiglifySettingsTests
    {
        [Fact]
        public async Task SerializationTest()
        {
            var expectedSettings = new MiglifySettings();

            const string path = "miglify.json";
            await File.WriteAllTextAsync(path, expectedSettings.ToJson());
            var file = await File.ReadAllTextAsync(path);
            var actualSettings = file.To<MiglifySettings>();

            Assert.True(expectedSettings.PublicInstancePropertiesEqual(actualSettings));
        }

        [Fact]
        public void GlobsFunctionAsDesired()
        {
            var defaultSettings = new MiglifySettings();

            var cssFiles = "wwwroot".Glob(defaultSettings.Globs[MiglifyType.Css].Input).ToList();

            Assert.True(cssFiles.Count == 2);
        }
    }
}