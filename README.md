# WinProxyViewer
Show Windows proxy settings. Requires .NET 4.5.

[![Build status](https://ci.appveyor.com/api/projects/status/j9s0ah707pv0xt7b?svg=true)](https://ci.appveyor.com/project/xmedeko/winproxyviewer)

## Usage
By default, the application writes the result to the console.

````
WinProxyViewer.exe [-h] [-tmp] [-out] [-o filename]
````

`-h` Writes help to console enxt exits.

`-tmp` Writes the result to the temporaty file and opens it. (Default option)

`-out` Writer the resoult to the console output.

`-o filename` Writes the result to the specified file.
