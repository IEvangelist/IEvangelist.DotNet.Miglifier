namespace IEvangelist.DotNet.Miglifier
{
    static class Constants
    {
        internal const string Name = "dotnet miglifier";

        internal const string FullName = "dotnet-miglifier";

        internal const string Description = "Minifies and Uglifies JavaScript, CSS and HTML files in place.";

        internal const string HelpText = @"
You could optionally to provide the path to the web-root (commonly named 'wwwroot') directory. Additionally, you are able to provide a the miglify.json file path.

Example:
miglifier ""wwwroot"" ""../../config/miglify.json"" ";

        internal const string PathName = "path";

        internal const string PathDescription = "Path to the web-root (commonly named 'wwwroot') directory.";

        internal const string MiglifyJsonPathName = "miglify-json";

        internal const string MiglifyJsonPathDescription = "Path to the miglify.json file.";
    }
}