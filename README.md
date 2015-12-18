# WinProxyViewer
Show Windows proxy settings. Requires .NET 4.5.

This small utility prints the Windows proxy settings as text, so it may be send by email, chat, etc.

[![Build status](https://ci.appveyor.com/api/projects/status/j9s0ah707pv0xt7b?svg=true)](https://ci.appveyor.com/project/xmedeko/winproxyviewer)

## Usage
````
WinProxyViewer.exe [-h] [-tmp] [-out] [-o filename]
````

`-h` Writes help to console and exits.

`-tmp` Writes the result to the temporary file and opens it. (Default option)

`-out` Writes the result to the console output.

`-o filename` Writes the result to the specified file.
