using NUglify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        internal static async Task<MiglifyResult> MiglifyAsync(string wwwroot)
        {
            if (!Directory.Exists(wwwroot))
            {
                throw new ArgumentException("The given directory does not exist.", nameof(wwwroot));
            }

            try
            {
                WriteLine("Scanning for CSS, JavaScript and HTML files to process.", ConsoleColor.Cyan);
                WriteLine();

                var results =
                    GetCssFiles(wwwroot)
                       .Select(css => new MiglifyFile(TargetType.Css, css))
                       .Concat(GetJsFiles(wwwroot)
                                  .Select(js => new MiglifyFile(TargetType.Js, js)))
                       .Concat(GetHtmlFiles(wwwroot)
                                  .Select(html => new MiglifyFile(TargetType.Html, html)));

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
                        var minifiedPath = Path.Combine(info.DirectoryName, minifiedName);

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

        static IEnumerable<FileInfo> GetAllFiles(string wwwroot, string searchPattern) 
            => Directory.EnumerateFiles(wwwroot, searchPattern, Options)
                        .Select(file => new FileInfo(file));

        static IEnumerable<string> GetCssFiles(string wwwroot)
            => GetFilesByExtension(wwwroot, CascadingStyleSheetsExtension);

        static IEnumerable<string> GetJsFiles(string wwwroot)
            => GetFilesByExtension(wwwroot, JavaScriptExtension);

        static IEnumerable<string> GetHtmlFiles(string wwwroot)
            => GetFilesByExtension(wwwroot, HyperTextMarkupLanguageExtension);

        static IEnumerable<string> GetFilesByExtension(string wwwroot, string extension)
            => GetAllFiles(wwwroot, $"*{extension}")
                   .Where(file => !file.Name.Contains(".min.", StringComparison.OrdinalIgnoreCase))
                   .Select(file => file.FullName);

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