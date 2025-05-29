using System;
using System.Threading.Tasks;
using Windows.Win32;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using yamvu;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Extensions.WebView;



namespace MinimalWebViewCounterSample;

class EntryPoint {

   private const string WindowTitle = "yamvu MinimalWebView Sample";
   private const uint BackgroundColor = 0x271811; // this is actually #111827, Windows uses BBGGRR


   [STAThread]
   static int Main() {
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

      ILogger? servicesLogger = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");
      ILogger? programLogger = loggerFactory?.CreateLogger("prog");

      int exitCode = Window.CreateAndShow(WindowTitle, BackgroundColor,
                                          afterShowWindowFunc: hwnd => {

                                                                  WebView.InitController(hwnd, handleMessageFromWebViewAsync, uiLogger);

                                                                  WebView webView = new();
                                                                  int exitCode = WebViewMvuHost.RunMvuApp(handleMessageFromWebViewAsyncDelegate: (string messageReceived, Func<string, Task> asyncAction, Action<IMvuMessage> dispatchMessage) => {
                                                                     // TODO: prevent injection attacks
                                                                     Message message = deserializeMessage(messageReceived);
                                                                     dispatchMessage(message);
                                                                                                                                                 }
                                                                                                          ,
                                                                                                          () => MvuMessages.Request_Quit(),

                                                                                                          // ReSharper disable once AccessToDisposedClosure
                                                                                                          () => Component.GetAsComponent(new AppServices_Real(servicesLogger),
                                                                                                                                         (dispatch, model) => ViewBuilder.BuildView(dispatch, model, uiLogger),
                                                                                                                                         programLogger,
                                                                                                                                         loggerFactory),
                                                                                                          loggerFactory);
                                                                  return exitCode;

                                                               });



      static Task handleMessageFromWebViewAsync(string webMessageReceived, Func<string, Task> executeScriptAsyncAction) {

      }






   }


}
