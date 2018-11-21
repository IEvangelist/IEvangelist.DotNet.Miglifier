namespace IEvangelist.DotNet.Miglifier
{
    static class Constants
    {
        internal const string Name = "dotnet miglifier";

        internal const string FullName = "dotnet-miglifier";

        internal const string Description = "Minifies and Uglifies JavaScript, CSS and HTML files in place.";

        internal const string HelpText = @"
You are required to provide the path to the web-root (commonly named 'wwwroot') directory.

Example:
miglifier ""wwwroot"" ";

        internal const string PathErrorMessage = "Please specify the path to the web-root (commonly named 'wwwroot') directory.";

        internal const string PathName = "path";

        internal const string PathDescription = "Path to the web-root (commonly named 'wwwroot') directory.";
    }
}