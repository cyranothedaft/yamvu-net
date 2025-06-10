using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Win32;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MinimalWebViewSample.Lib.Console;
using MinimalWebViewSample.Lib.WebView;
using MinimalWebViewSample.Lib.Window;
using yamvu;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Extensions.WebView;



namespace MinimalWebViewCounterSample;

class EntryPoint {

   private const string WindowTitle = "yamvu MinimalWebView Sample";
   private const int WindowWidth = 800;
   private const int WindowHeight = 600;
   private const uint BackgroundColor = 0x271811; // this is actually #111827, Windows uses BBGGRR


   [STAThread]
   static int Main() {

#if DEBUG // By default GUI apps have no console. Open one to enable Console.WriteLine debugging 🤠
      MinimalConsole.OpenConsole();
#endif

      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                                                                      builder
                                                                           .AddSimpleConsole(options => {
                                                                                                options.IncludeScopes   = true;
                                                                                                options.SingleLine      = true;
                                                                                                options.TimestampFormat = "HH:mm:ss ";
                                                                                                options.ColorBehavior   = LoggerColorBehavior.Enabled;
                                                                                             })
                                                                           .AddFilter("bus", LogLevel.Debug)
                                                                           .AddFilter("WM" , LogLevel.Debug)
                                                                           .SetMinimumLevel( LogLevel.Trace) // fallback/default
                                                               );

      ILogger? appLogger         = loggerFactory?.CreateLogger("app");
      ILogger? servicesLogger    = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger          = loggerFactory?.CreateLogger("UI");
      ILogger? messagePumpLogger = loggerFactory?.CreateLogger("WM");
      ILogger? programLogger     = loggerFactory?.CreateLogger("prog");

      ILogger? windowLogger = loggerFactory?.CreateLogger("win");
      ILogger? webViewLogger = loggerFactory?.CreateLogger("web");

      (MinimalWindow window, MinimalWebView webView) = MinimalWebViewSample.Lib.WebViewWindow.Create(WindowTitle, WindowWidth, WindowHeight, BackgroundColor,
                                                                                                     windowLogger, webViewLogger);

      WebViewWindow webViewWindow = new WebViewWindow(window, webView);
      webViewWindow.AttachMvuProgram(MvuMessages.Request_Quit,
                                     () => Component.GetAsComponent(new AppServices_Real(servicesLogger),
                                                                    (dispatch, model) => ViewBuilder.BuildView(dispatch, model, uiLogger),
                                                                    programLogger,
                                                                    loggerFactory),
                                     webMessage => deserializeMessage(webMessage, appLogger),
                                     appLogger, loggerFactory);
      window.Show();
      int exitCode = MessagePump.Run(messagePumpLogger);

#if DEBUG
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine($"Ended with exit code {exitCode}.");
      Console.WriteLine("Press enter to exit.");
      Console.ReadLine();
#endif
      return exitCode;
   }


   // TODO?
   //    see:  https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#logging-in-a-non-trivial-app
   //    and:  https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator
   // [LoggerMessage(Level = LogLevel.Information, Message = "Hello World! Logging is {Description}.")]
   // static partial void LogStartupMessage(ILogger logger, string description);

   private static IMvuMessage deserializeMessage(string webMessage, ILogger? appLogger) {
      appLogger?.LogTrace("### web message [{str}]", webMessage);

      if (webMessage.StartsWith("msg:")) {
         switch (webMessage[4..]) {
            case "increment1":      return MvuMessages.Request_Increment1();
            case "incrementrandom": return MvuMessages.Request_IncrementRandom();
         }
      }
      throw new NotImplementedException($"message not handled: [{webMessage}]");
   }
}
