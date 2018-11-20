using NUglify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        internal static MiglifyResult Miglify(string wwwroot, IConsole console = null)
        {
            if (!Directory.Exists(wwwroot))
            {
                throw new ArgumentException("The given directory does not exist.", nameof(wwwroot));
            }

            console = console ?? PhysicalConsole.Singleton;

            try
            {
                var results =
                    GetCssFiles(wwwroot)
                        .Select(css => (path: css, type: TargetType.Css, result: Uglify.Css(File.ReadAllText(css))))
                        .Concat(GetJsFiles(wwwroot).Select(js => (path: js, type: TargetType.Js, result: Uglify.Js(File.ReadAllText(js))))
                        .Concat(GetHtmlFiles(wwwroot).Select(html => (path: html, type: TargetType.Html, result: Uglify.Html(File.ReadAllText(html))))));

                bool IsErrorFree(UglifyResult result)
                {
                    result.Errors?.ForEach(console.Error.WriteLine);
                    return !result.HasErrors;
                };

                var buffer = new List<(string InputPath, string OutputPath, TargetType Type)>();
                foreach (var (path, type, result) in results.Where(_ => IsErrorFree(_.result)))
                {
                    var file = new FileInfo(path);
                    var minifiedName = $"{Path.GetFileNameWithoutExtension(file.Name)}.min{file.Extension}";
                    var minifiedPath = Path.Combine(file.DirectoryName, minifiedName);

                    File.WriteAllText(minifiedPath, result.Code);
                    buffer.Add((path, minifiedPath, type));
                    console.WriteLine($"Minified ${type}: {minifiedPath}");
                }

                return new MiglifyResult(0, buffer);
            }
            catch (Exception ex)
            {
                console.Error.WriteLine(ex);
                return new MiglifyResult(2);
            }
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
    }
}