using NUglify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IEvangelist.DotNet.Miglifier.Config;
using IEvangelist.DotNet.Miglifier.Extensions;
using McMaster.Extensions.CommandLineUtils;

namespace IEvangelist.DotNet.Miglifier.Core
{
    static class MinifierAndUglifier
    {
        const string CascadingStyleSheetsExtension = ".css";
        const string JavaScriptExtension = ".js";
        const string HyperTextMarkupLanguageExtension = ".html";

        static readonly EnumerationOptions Options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            ReturnSpecialDirectories = false,
            MatchCasing = MatchCasing.CaseInsensitive
        };

        static IConsole Console { get; } = PhysicalConsole.Singleton;

        internal static async Task<MiglifyResult> MiglifyAsync(string wwwroot, string miglifyJson)
        {
            if (!Directory.Exists(wwwroot))
            {
                throw new ArgumentException("The given directory does not exist.", nameof(wwwroot));
            }

            if (!string.IsNullOrWhiteSpace(miglifyJson) && !File.Exists(miglifyJson))
            {
                throw new ArgumentException("The given miglify.json file path does not exist.",
                                            nameof(miglifyJson));
            }

            try
            {
                WriteLine("Scanning for CSS, JavaScript and HTML files to process.", ConsoleColor.Cyan);
                WriteLine();

                var settings = await LoadMiglifySettingsAsync(miglifyJson);
                var results = GetMiglifiedFiles(wwwroot, settings);

                var buffer = new List<MiglifyFile>();
                foreach (var (type, files) in results.GroupBy(f => f.Type)
                                                     .ToDictionary(grp => grp.Key, grp => grp.ToList()))
                {
                    WriteLine($"Processing {files.Count} {type} file(s).");

                    foreach (var file in files)
                    {
                        var (path, resultTask) = file;
                        var result = await resultTask;
                        if (!IsErrorFree(result))
                        {
                            continue;
                        }

                        var info = new FileInfo(path);
                        var minifiedName = $"{Path.GetFileNameWithoutExtension(info.Name)}.min{info.Extension}";
                        var directorySettings = settings.Globs[file.Type];
                        var outputPath = (directorySettings.Output ?? info.DirectoryName).ToFullPath();
                        Directory.CreateDirectory(outputPath);
                        var minifiedPath = Path.Combine(outputPath, minifiedName);

                        await File.WriteAllTextAsync(file.MiglifiedPath = minifiedPath, result.Code);

                        buffer.Add(file);
                        WriteLine($"\t{minifiedPath}", ConsoleColor.DarkCyan);
                    }

                    WriteLine();
                }

                WriteLine($"Successfully miglified {buffer.Count} files!", ConsoleColor.Cyan);

                return new MiglifyResult(0, buffer);
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return new MiglifyResult(2);
            }
        }

        static IEnumerable<MiglifyFile> GetMiglifiedFiles(string wwwroot, MiglifySettings settings)
            => wwwroot.Glob(settings.Globs[MiglifyType.Css].Input)
                      .Select(css =>
                              new MiglifyFile(
                                  MiglifyType.Css,
                                  css,
                                  file => Uglify.Css(file, settings.CssSettings)))
                      .Concat(
                           wwwroot.Glob(settings.Globs[MiglifyType.Js].Input)
                                  .Select(js =>
                                          new MiglifyFile(
                                              MiglifyType.Js,
                                              js,
                                              file => Uglify.Js(file, settings.JavaScriptSettings))))
                      .Concat(
                           wwwroot.Glob(settings.Globs[MiglifyType.Html].Input)
                                  .Select(html =>
                                          new MiglifyFile(
                                              MiglifyType.Html,
                                              html,
                                              file => Uglify.Html(file, settings.HtmlSettings))));

        static async Task<MiglifySettings> LoadMiglifySettingsAsync(string miglifyJson)
        {
            if (string.IsNullOrWhiteSpace(miglifyJson) || !File.Exists(miglifyJson))
            {
                return new MiglifySettings();
            }

            var json = await File.ReadAllTextAsync(miglifyJson);
            return json.To<MiglifySettings>();
        }

        static bool IsErrorFree(UglifyResult result)
        {
            result.Errors?.ForEach(e =>
            {
                if (e.IsError)
                {
                    WriteError($"\t{e}");
                }
                else
                {
                    WriteLine($"\t{e}", ConsoleColor.DarkYellow);
                }
            });

            return !result.HasErrors;
        }
        
        static void WriteLine(string message = null, ConsoleColor? color = null)
        {
            try
            {
                Console.ForegroundColor = color ?? Console.ForegroundColor;
                if (message is null)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
            finally
            {
                Console.ResetColor();
            }
        }

        static void WriteError(object message)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}