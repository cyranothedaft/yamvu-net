using System;
using Windows.Win32;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;



namespace MinimalWebView;

class EntryPoint {

   private const string WindowTitle = "yamvu MinimalWebView Sample";
   private const uint BackgroundColor = 0x271811; // this is actually #111827, Windows uses BBGGRR


   [STAThread]
   static int Main(string[] args) {

#if DEBUG // By default GUI apps have no console. Open one to enable Console.WriteLine debugging 🤠
      PInvoke.AllocConsole();
#endif

      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                                                                      builder.AddSimpleConsole(options => {
                                                                                                  options.IncludeScopes   = true;
                                                                                                  options.SingleLine      = true;
                                                                                                  options.TimestampFormat = "HH:mm:ss ";
                                                                                                  options.ColorBehavior   = LoggerColorBehavior.Enabled;
                                                                                               })
                                                                             .SetMinimumLevel(LogLevel.Trace));
      var logger = loggerFactory.CreateLogger("");

      int exitCode = WebViewMessagePumpWindow.Run(WindowTitle, BackgroundColor, logger);
      return exitCode;

   }
}
