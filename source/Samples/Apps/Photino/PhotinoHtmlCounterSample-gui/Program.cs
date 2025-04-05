using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CounterMvu_lib;
using CounterMvu_lib.Effects;
using CounterMvu_lib.Messages;
using CounterSample.AppCore;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using Photino.NET;
using yamvu;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Runners;
using CounterProgram = CounterMvu_lib.Program.Program;



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
      IMvuProgramRunner<PhotinoView> mvuProgramRunner = buildMvuProgramRunner(loggerFactory);
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
                                      .SetContextMenuEnabled(false)
                                      .SetDevToolsEnabled(false)
             // .RegisterDynamicScript()
             // .RegisterWebMessageHandler()
            ;
      return window;
   }


   private static IMvuProgram2<Model, PhotinoView> buildMvuProgram(ILoggerFactory? loggerFactory) {
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");
      ILogger? programLogger = loggerFactory?.CreateLogger("prog");
      return CounterProgram.Build((dispatch, model) => ViewBuilder.BuildView(dispatch, model, uiLogger), programLogger);
   }


   private static IMvuProgramRunner<PhotinoView> buildMvuProgramRunner(ILoggerFactory? loggerFactory)
      => CounterProgram.BuildRunner<PhotinoView>(loggerFactory);


   private static void attachMvuProgramToWindow(PhotinoWindow window, IAppServices appServices, IMvuProgramRunner<PhotinoView> mvuProgramRunner, IMvuProgram2<Model, PhotinoView> mvuProgram,
                                                ExternalMessageDispatcher externalMessageDispatcher, Action<Exception> handleException,
                                                ILogger? appLogger, ILoggerFactory? loggerFactory) {
      window.WindowClosing += (sender, args) => {
                                 appLogger?.LogTrace("# WindowClosing");
                                 externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
                                 return false; // let it close
                              };
      window.WebMessageReceived += handleWebMessage;

      return;

      async void handleWebMessage(object? sender, string str) {
         try {
            appLogger?.LogTrace("### web message [{str}]", str);
            if (str == "StartMvuProgram") {
               await runMvuProgramAsync1();
            }
            else if (str.StartsWith("msg:")) {
               externalMessageDispatcher.Dispatch(str[4..] switch
                  {
                     "increment1"      => MvuMessages.Request_Increment1(),
                     "incrementrandom" => MvuMessages.Request_IncrementRandom(),
                     _                 => throw new ArgumentOutOfRangeException()
                  });
            }
         }
         catch (Exception exception) {
            handleException(exception);
         }
      }

      async Task runMvuProgramAsync1() {
         appLogger?.LogTrace(">> MVU program");
         await runMvuProgramAsync(window, appServices, mvuProgramRunner, mvuProgram,
                                  externalMessageDispatcher, loggerFactory);
         appLogger?.LogTrace("<< MVU program");

         // the MVU program has ended, so signal the window to close (if it's not closed already)
         appLogger?.LogTrace(">> Closing window");
         window.Close();
         appLogger?.LogTrace("<< Closing window");
      }
   }


   private static async Task<Model> runMvuProgramAsync(PhotinoWindow window, IAppServices appServices, IMvuProgramRunner<PhotinoView> mvuProgramRunner, IMvuProgram2<Model, PhotinoView> mvuProgram,
                                                       ExternalMessageDispatcher? externalMessageDispatcher, ILoggerFactory? loggerFactory) {
      ILogger? effectLogger = loggerFactory?.CreateLogger("fx");
      IEffects effectExecutor = new EffectExecutor(appServices);
      Model finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuProgramRunner, mvuProgram, updateView1,
                                                                                 loggerFactory, externalMessageDispatcher, CounterProgram.Info,
                                                                                 CounterProgram.MessageAsCommandFunc, executeEffect1, CounterProgram.IsQuitMessageFunc);
      return finalModel;

      void updateView1(PhotinoView view)
         => updateView(window, view);

      void executeEffect1(IMvuEffect effect, IHasEffectResultHandler hasresulthandler)
         => executeEffect(effectExecutor, effect, hasresulthandler, effectLogger);
   }


   private static void updateView(PhotinoWindow window, PhotinoView view) {
      // string indexHtml = File.ReadAllText(Path.Combine(window.TemporaryFilesPath, "index.html")).Replace("==test==", view.HtmlFragment);
      // window.LoadRawString(indexHtml);
      window.SendWebMessage(view.HtmlFragment);
   }


   private static void executeEffect(IEffects effectExecutor, IMvuEffect effect, IHasEffectResultHandler resultHandler, ILogger? effectLogger) {
      // TODO: await !!!
      // this fires-and-forgets without proper exception trapping
      EffectDispatcher.DispatchToExecutorAsync(effectExecutor, effect, resultHandler, effectLogger);
   }


   private static void runWindow(PhotinoWindow window) {
      window = window.Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.
      window.WaitForClose(); // Starts the application event loop
   }


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
