using IEvangelist.DotNet.Miglifier.Core;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using System.Threading.Tasks;

namespace IEvangelist.DotNet.Miglifier
{
    [
        HelpOption,
        Command(
            Name = Constants.Name,
            FullName = Constants.FullName,
            Description = Constants.Description,
            ExtendedHelpText = Constants.HelpText)
    ]
    public class Miglifier
    {
        [
            FileOrDirectoryExists,
            Argument(0, Constants.PathName, Constants.PathDescription)
        ]
        public string Path { get; } = "wwwroot";

        [
            FileOrDirectoryExists,
            Argument(1, Constants.MiglifyJsonPathName, Constants.MiglifyJsonPathDescription)
        ]
        public string MiglifyJsonPath { get; }

        public async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (!Directory.Exists(Path))
            {
                app.ShowHelp();
                return 1;
            }

            var result = await MinifierAndUglifier.MiglifyAsync(Path, MiglifyJsonPath);
            return result.ExitCode;
        }
    }
}