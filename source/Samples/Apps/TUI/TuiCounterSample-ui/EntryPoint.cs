using System;
using Microsoft.Extensions.Logging;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using WelterKit.Telemetry.Logging.MinimalFile;


namespace TuiCounterSample.ui;

static class EntryPoint {
   private static void Main(string[] args) {

      // Override the default configuration for the application to use the Light theme
      //ConfigurationManager.RuntimeConfig = """{ "Theme": "Light" }""";
      ConfigurationManager.Enable(ConfigLocations.All);


      const LogLevel minimumLogLevel = LogLevel.Trace;

      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                                                             builder
                                                                  .AddFile((MinimalFileLoggerOptions options) => {
                                                                              options.LogFilePath     = @".\sample.log";
                                                                              options.ForceSingleLine = true;
                                                                           })

                                                                  .AddFilter("Program", LogLevel.Information)
                                                                  .SetMinimumLevel(LogLevel.Trace) // fallback/default
                                                                   ;
                                                          });
         ILogger? svcsLogger = loggerFactory?.CreateLogger("svcs");
         ILogger? uiLogger = loggerFactory?.CreateLogger("ui");

         new MainForm()
              .RunMvuApp(() => MvuMessages.Request_Quit(),
                         // ReSharper disable once AccessToDisposedClosure
                         () => getComponent(svcsLogger, uiLogger, loggerFactory),
                         loggerFactory);



      Application.Run<ExampleWindow>().Dispose();

      // Before the application exits, reset Terminal.Gui for clean shutdown
      Application.Shutdown();

      // To see this output on the screen it must be done after shutdown,
      // which restores the previous screen.
      Console.WriteLine($"Username: {ExampleWindow.UserName}");
   }


   private static MvuProgramComponent<Model, ProgramView> getComponent(ILogger? svcsLogger, ILogger? uiLogger, ILoggerFactory? loggerFactory)
      => Component.GetAsComponent(new AppServices_Real(svcsLogger),
                                  viewFunc: (dispatch, model) => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger),
                                  loggerFactory?.CreateLogger("prog"),
                                  loggerFactory);
}
