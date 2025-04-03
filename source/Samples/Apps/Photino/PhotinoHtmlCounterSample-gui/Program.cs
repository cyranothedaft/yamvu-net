using System;
using Photino.NET;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CounterMvu_lib;
using CounterMvu_lib.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using yamvu;
using yamvu.core;
using yamvu.Runners;



namespace PhotinoHtmlCounterSample.gui;

//NOTE: To hide the console window, go to the project properties and change the Output Type to Windows Application.
// Or edit the .csproj file and change the <OutputType> tag from "WinExe" to "Exe".
internal static class Program {
   [STAThread]
   static void Main(string[] args) {

      using ILoggerFactory loggerFactory = buildLoggerFactory(LogLevel.Trace);
      ILogger appLogger = loggerFactory.CreateLogger("app");
      AppServices_Real appServices = new AppServices_Real(loggerFactory?.CreateLogger("svcs"));
      ExternalMessageDispatcher externalMessageDispatcher = new();

      PhotinoWindow window = buildWindow();
      IMvuProgram2<Model, PhotinoView> mvuProgram = buildMvuProgram(loggerFactory);
      IMvuProgramRunner<PhotinoView> mvuProgramRunner = buildMvuProgramRunner(window, appServices, externalMessageDispatcher, loggerFactory);
      attachMvuProgramToWindow(window, appServices, mvuProgramRunner, mvuProgram, externalMessageDispatcher,
                               exception => appLogger?.LogError(exception, "Application error"),
                               appLogger, loggerFactory);
      runWindow(window);

   }


   private static PhotinoWindow buildWindow() {
      // Window title declared here for visibility
      string windowTitle = "Photino for .NET Demo App";

      // Creating a new PhotinoWindow instance with the fluent API
      var window = new PhotinoWindow().SetTitle(windowTitle)
                                       // Resize to a percentage of the main monitor work area
                                      .SetUseOsDefaultSize(false)
                                      .SetSize(new Size(1024, 800))
                                       // Center window in the middle of the screen
                                      .Center()
                                       // Users can resize windows by default.
                                       // Let's make this one fixed instead.
                                      .SetResizable(false)
            // .RegisterDynamicScript()
            // .RegisterWebMessageHandler1()
            ;
      return window;
   }


   private static IMvuProgram2<Model, PhotinoView> buildMvuProgram(ILoggerFactory? loggerFactory) {
      IMvuProgram2<Model, PhotinoView> program = MvuProgramRunner.BuildMvuProgram(loggerFactory);
      return program;
   }


   private static IMvuProgramRunner<PhotinoView> buildMvuProgramRunner(PhotinoWindow window, IAppServices services, ExternalMessageDispatcher? externalMessageDispatcher, ILoggerFactory? loggerFactory) {
      IMvuProgramRunner<PhotinoView> program = MvuProgramRunner.BuildMvuProgramRunner(window, services, externalMessageDispatcher, loggerFactory);
      return program;
   }


   private static void attachMvuProgramToWindow(PhotinoWindow window, IAppServices appServices, IMvuProgramRunner<PhotinoView> mvuProgramRunner, IMvuProgram2<Model, PhotinoView> mvuProgram,
                                                ExternalMessageDispatcher? externalMessageDispatcher, Action<Exception> handleException,
                                                ILogger? appLogger, ILoggerFactory? loggerFactory) {
      window.WindowClosing += (sender, args) => {
                                 appLogger?.LogTrace("# WindowClosing");
                                 externalMessageDispatcher?.Dispatch(MvuMessages.Request_Quit());
                                 return false; // let it close
                              };
      window.WebMessageReceived += handleWebMessage;

      async void handleWebMessage(object? sender, string str) {
         try {
            appLogger?.LogTrace("### web message [{str}]", str);
            if (str == "mvu:StartProgram") {
               window.WebMessageReceived -= handleWebMessage;
               await runMvuProgramAsync();
            }
         }
         catch (Exception exception) {
            handleException(exception);
         }
      }

      async Task runMvuProgramAsync() {
         appLogger?.LogTrace(">> MVU program");
         await MvuProgramRunner.RunMvuProgramAsync(window, appServices, mvuProgramRunner, mvuProgram,
                                                   externalMessageDispatcher, loggerFactory);
         appLogger?.LogTrace("<< MVU program");

         // the MVU program has ended, so signal the window to close (if it's not closed already)
         appLogger?.LogTrace(">> Closing window");
         window.Close();
         appLogger?.LogTrace("<< Closing window");
      }
   }


   private static void runWindow(PhotinoWindow window) {
      window = window.Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.
      window.WaitForClose(); // Starts the application event loop
   }


   private static PhotinoWindow RegisterDynamicScript(this PhotinoWindow window)
      => window.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) => {
                                                      contentType = "text/javascript";
                                                      return new MemoryStream(Encoding.UTF8.GetBytes(@"
                                                          (() =>{
                                                              window.setTimeout(() => {
                                                                  alert(`🎉 Dynamically inserted JavaScript.`);
                                                              }, 1000);
                                                          })();
                                                      "));
                                                   });


   private static PhotinoWindow RegisterWebMessageHandler1(this PhotinoWindow window)
      => window
            // Most event handlers can be registered after the
            // PhotinoWindow was instantiated by calling a registration 
            // method like the following RegisterWebMessageReceivedHandler.
            // This could be added in the PhotinoWindowOptions if preferred.
           .RegisterWebMessageReceivedHandler((object sender, string message) => {
                                                 if (message.StartsWith("mvu:")) return;

                                                 var windowSender = (PhotinoWindow)sender;

                                                 // The message argument is coming in from sendMessage.
                                                 // "window.external.sendMessage(message: string)"
                                                 string response = $"Received message: \"{message}\"";

                                                 // Send a message back the to JavaScript event handler.
                                                 // "window.external.receiveMessage(callback: Function)"
                                                 windowSender.SendWebMessage(response);
                                              });



   private static ILoggerFactory buildLoggerFactory(LogLevel minimumLogLevel)
      => LoggerFactory.Create(builder =>
                                    builder.AddDebug()
                                           .SetMinimumLevel(minimumLogLevel));

}
