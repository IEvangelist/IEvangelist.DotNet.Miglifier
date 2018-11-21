using IEvangelist.DotNet.Miglifier.Core;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
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
            Required(ErrorMessage = Constants.PathErrorMessage),
            Argument(0, Constants.PathName, Constants.PathDescription)
        ]
        public string Path { get; }

        public async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (!Directory.Exists(Path))
            {
                app.ShowHelp();
                return 1;
            }

            var result = await MinifierAndUglifier.MiglifyAsync(Path);
            return result.ExitCode;
        }
    }
}