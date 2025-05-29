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
using yamvu.Extensions.WebView.Library;
using yamvu.Extensions.WebView.Library.WebView;
using yamvu.Extensions.WebView.Library.Window;



namespace MinimalWebViewCounterSample;

class EntryPoint {

   private const string WindowTitle = "yamvu MinimalWebView Sample";
   private const int WindowWidth = 800;
   private const int WindowHeight = 600;
   private const uint BackgroundColor = 0x271811; // this is actually #111827, Windows uses BBGGRR


   [STAThread]
   static int Main() {

#if DEBUG // By default GUI apps have no console. Open one to enable Console.WriteLine debugging 🤠
      MinimalConsole.ShowConsole();
#endif

      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                                                                      builder.AddSimpleConsole(options => {
                                                                                                  options.IncludeScopes   = true;
                                                                                                  options.SingleLine      = true;
                                                                                                  options.TimestampFormat = "HH:mm:ss ";
                                                                                                  options.ColorBehavior   = LoggerColorBehavior.Enabled;
                                                                                               })
                                                                             .SetMinimumLevel(LogLevel.Debug));

      ILogger? servicesLogger = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger = loggerFactory?.CreateLogger("UI");
      ILogger? messagePumpLogger = loggerFactory?.CreateLogger("WM");
      ILogger? programLogger = loggerFactory?.CreateLogger("prog");


      MinimalWindow window = MinimalWindow.Create(WindowTitle, WindowWidth, WindowHeight, BackgroundColor, uiLogger);
      window.Show();

      MinimalWebView webView = MinimalWebView.Init(window, uiLogger);
      window.SizeChanged += webView.SetSize;

      webView.EmbedMvuProgram(MvuMessages.Request_Quit, 
                               () => Component.GetAsComponent(new AppServices_Real(servicesLogger), 
                                                              (dispatch, model) => ViewBuilder.BuildView(dispatch, model, uiLogger),
                                                              programLogger,
                                                              loggerFactory),
                               loggerFactory);


      int exitCode = MessagePump.Run(messagePumpLogger);
      return exitCode;

      // int exitCode = Window.CreateAndShow(WindowTitle, BackgroundColor,
      //                                     afterShowWindowFunc: hwnd => {
      //
      //                                                             WebView.InitController(hwnd, handleMessageFromWebViewAsync, uiLogger);
      //
      //                                                             WebView webView = new();
      //                                                             int exitCode = WebViewMvuHost.RunMvuApp(handleMessageFromWebViewAsyncDelegate: (string messageReceived, Func<string, Task> asyncAction, Action<IMvuMessage> dispatchMessage) => {
      //                                                                // TODO: prevent injection attacks
      //                                                                Message message = deserializeMessage(messageReceived);
      //                                                                dispatchMessage(message);
      //                                                                                                                                            }
      //                                                                                                     ,
      //                                                                                                     () => MvuMessages.Request_Quit(),
      //
      //                                                                                                     // ReSharper disable once AccessToDisposedClosure
      //                                                                                                     () => Component.GetAsComponent(new AppServices_Real(servicesLogger),
      //                                                                                                                                    (dispatch, model) => ViewBuilder.BuildView(dispatch, model, uiLogger),
      //                                                                                                                                    programLogger,
      //                                                                                                                                    loggerFactory),
      //                                                                                                     loggerFactory);
      //                                                             return exitCode;
      //
      //                                                          });
   }


}
