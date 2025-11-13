using System;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Views;
using TuiCounterSample.ui.View;
using UI;
using WelterKit.Telemetry.Logging.MinimalFile;
using yamvu;
using yamvu.Extensions.Tui;


namespace TuiCounterSample.ui;

static class EntryPoint {
   private static void Main(string[] args) {

      // Override the default configuration for the application to use the Light theme
      //ConfigurationManager.RuntimeConfig = """{ "Theme": "Light" }""";
      ConfigurationManager.Enable(ConfigLocations.All);

      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                                                                   builder.AddFile(options => {
                                                                                      options.LogFilePath     = @".\TuiCounterSample-ui.log";
                                                                                      options.ForceSingleLine = true;
                                                                                   })

                                                                          .AddFilter("Program", LogLevel.Information)
                                                                          .SetMinimumLevel(LogLevel.Trace) // fallback/default
                                                                         ;
                                                                });

      ILogger? svcsLogger = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");

      Application.Init();

      Model finalModel;
      using ( var mainWindow = new MainWindow() ) {
         TuiMvuHost.RunMvuApp(mainWindow,
                              () => MvuMessages.Request_Quit(),
                              // ReSharper disable once AccessToDisposedClosure
                              () => getComponent(svcsLogger, uiLogger, loggerFactory),
                              loggerFactory);
         finalModel = mainWindow.FinalModel;
      }


      // Before the application exits, reset Terminal.Gui for clean shutdown
      Application.Shutdown();

      // To see this output on the screen it must be done after shutdown,
      // which restores the previous screen.
      Console.WriteLine($"Final value: {finalModel}");
   }


   private static MvuProgramComponent<Model, ProgramView> getComponent(ILogger? svcsLogger, ILogger? uiLogger, ILoggerFactory? loggerFactory)
      => Component.GetAsComponent(new AppServices_Real(svcsLogger),
                                  viewFunc: (dispatch, model) => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger),
                                  loggerFactory?.CreateLogger("prog"),
                                  loggerFactory);
}
