using System.Collections.Generic;
using IEvangelist.DotNet.Miglifier.Core;
using NUglify.Css;
using NUglify.Html;
using NUglify.JavaScript;

namespace IEvangelist.DotNet.Miglifier.Config
{
    public class MiglifySettings
    {
        public Dictionary<MiglifyType, DirectorySettings> Globs { get; set; } =
            new Dictionary<MiglifyType, DirectorySettings>
            {
                [MiglifyType.Css] = new DirectorySettings { Input = "**/*.css" },
                [MiglifyType.Html] = new DirectorySettings { Input = "**/*.html" },
                [MiglifyType.Js] = new DirectorySettings { Input = "**/*.js" }
            };

        public CodeSettings JavaScriptSettings { get; set; } = new CodeSettings();

        public CssSettings CssSettings { get; set; } = new CssSettings();

        public HtmlSettings HtmlSettings { get; set; } = new HtmlSettings();
    }

    public class DirectorySettings
    {
        public string Input { get; set; }

        public string Output { get; set; }
    }
}