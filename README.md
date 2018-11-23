# Status

[![NuGet version (dotnet-miglifier)](https://img.shields.io/nuget/v/dotnet-miglifier.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-miglifier/)
[![Build status](https://dev.azure.com/davidpine/IEvangelist.DotNet.Miglifier/_apis/build/status/IEvangelist.DotNet.Miglifier%20.NET%20Core-CI)](https://dev.azure.com/davidpine/IEvangelist.DotNet.Miglifier/_build/latest?definitionId=4)

# The "Miglifier" Explained 😅

Obviously "miglifier" is a made up word but it's fun to say...nonetheless, it still serves a purpose. This __Global .NET Tool__ both "minifies" and "uglifies" thus it "miglifies" CSS, JavaScript and HTML files in-place. I've yet to add bundling configuration or anything like that at this point. Nor, are there any options for how to perform various optimizations. It simply relys on all the defaults, more to come as time permits. But for now, this can be used as a part of your build tooling to minify and uglify files in-place. The files are _not_ re-located.

## Getting Started

To install the tool globally, execute the following command.

```console
dotnet tool install -g dotnet-miglifier
```

Once installed you may invoke the tool by simply calling its name and passing in the path for which you want it to "miglify" against, see below for example:

```console
dotnet-miglifier "wwwroot"
Scanning for CSS, JavaScript and HTML files to process.
Processing 1 Css file(s).
        .\wwwroot\main.min.css

Processing 1 Js file(s).
        .\wwwroot\main.min.js

Processing 1 Html file(s).
        .\wwwroot\main.min.html

Successfully miglified 3 files!
```

If there are errors or warnings 💩, those will be output as part of the execution of this tool. Enjoy responsibly 🤘!
