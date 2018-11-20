using IEvangelist.DotNet.Miglifier.Core;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IO;

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
            Argument(0, Name = Constants.PathName, Description = Constants.PathDescription)
        ]
        public string Path { get; }

        public int OnExcute(CommandLineApplication app, IConsole console)
        {
            if (!Directory.Exists(Path))
            {
                app.ShowHelp();
                return 1;
            }

            var result = MinifierAndUglifier.Miglify(Path, console);
            return result.ExitCode;
        }
    }
}