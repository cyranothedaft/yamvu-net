# yamvu-net - (yet another) MVU Framework, for .NET

Made from scratch using only the freshest organic ingredients (blood, sweat, tears) and huge amounts of research, trial, and error.

## Consists of these libraries (currently):


- ### yamvu 

  The main MVU library; mainly it contains the program "runners".

  _Targets .NET 8.0_.

  https://www.nuget.org/packages/yamvu/

  `dotnet add package yamvu --version 0.4.0-beta`


- ### yamvu.Core

  Core MVU types and abstractions.

  _Targets .NET 8.0_.

  https://www.nuget.org/packages/yamvu.Core/

  `dotnet add package yamvu.Core --version 0.4.0-beta`


- ### yamvu.Extensions.WinForms

  Adapts a WinForms UI for using yamvu. References MinimalWebViewLib:
   - github: https://github.com/cyranothedaft/MinimalWebViewLib
   - nuget: https://www.nuget.org/packages/MinimalWebViewLib/

  _Targets .NET 8.0 + Windows_.

  https://www.nuget.org/packages/yamvu.Extensions.WebView/

  `dotnet add package yamvu.Extensions.WebView --version 0.4.0-beta`


- ### yamvu.Extensions.WebView

  Adapts a WebView UI for using yamvu.

  _Targets .NET 8.0 + Windows_.

  https://www.nuget.org/packages/yamvu.Extensions.WinForms/

  `dotnet add package yamvu.Extensions.WinForms --version 0.4.0-beta`


- ### yamvu.Testing

  Utilities to aid with automated testing / unit tests.

  _Targets .NET 8.0_.

  https://www.nuget.org/packages/yamvu.Testing/

  `dotnet add package yamvu.Testing --version 0.4.0-beta`



_This is Work in progress. It is probably not for everyone._

> Many thanks to those who have written about or created existing MVU platforms, concepts, and related ideas. I'm keeping track and will list those acknowledgements here when I have time.
