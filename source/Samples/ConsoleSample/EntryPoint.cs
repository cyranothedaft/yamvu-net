using System;
using ConsoleSample.AppEvents;
using ConsoleSample.PlatAgnAppCore;
using ConsoleSample.UIBasics;
using Microsoft.Extensions.Logging;


namespace ConsoleSample;

internal static class EntryPoint {
   internal static async Task Main(string[] args) {
      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                                  .SetMinimumLevel(minimumLogLevel));

      using ConsoleSurface consoleSurface = new(10, 10);


      ILogger uiLogger = loggerFactory.CreateLogger("ui");
      ILogger appLogger = loggerFactory.CreateLogger("app");

      // define MVU program and wire up message loop
      ( _,
        AppEventSinks appEventSinks,
        ProgramEventSources programInputSources
      ) = wireUpEvents(uiLogger);
      consoleSurface.WireUpKeyPressEvents(appEventSinks);

      AppMain<View> appMain = AppMain<View>.Build(programInputSources,
                                                  (appLogger,
                                                   loggerFactory.CreateLogger("svc"),
                                                   loggerFactory.CreateLogger("run"),
                                                   loggerFactory.CreateLogger("prg"),
                                                   loggerFactory.CreateLogger("fx"),
                                                   loggerFactory.CreateLogger("bus"))
                                                 );

      consoleSurface.StartMonitoringKeyboard();

      appLogger?.LogInformation("Started, running program to completion...");
      try {
         consoleSurface.DrawView(ViewBuilder.BuildInitialView());
         await appMain.RunProgramWithCommonBusAsync(replaceViewAction: consoleSurface.DrawView,
                                                    viewFunc: ViewBuilder.BuildFromModel);

         appLogger?.LogInformation("Program terminated normally - application will now end.");
      }
      catch (Exception exception) {
         // TODO ===
         await Console.Error.WriteLineAsync(exception.ToString());
      }
   }


   private static (AppEventSources, AppEventSinks, ProgramEventSources) wireUpEvents(ILogger? uiLogger) {
      // wire up event dispatchers
      AppEventSources appEventSources = new(uiLogger);
      AppEventSinks appEventSinks = new(handleAppKeyPressed: appEventSources.RaiseAppKeyPressed, uiLogger);

      ProgramEventSources programInputSources = new(uiLogger);

      // translate from app-level input event to program-level input event
      appEventSources.AppKeyPressed += args => {
                                          if (isQuitKey(args)) {
                                             ((IProgramEventSource_QuitButtonPressed)programInputSources).RaiseQuitButtonPressed();
                                             return true; // handled
                                          }
                                          // else if (isRefreshKey(args)) {
                                          //    ((IProgramEventSource_RefreshButtonPressed)programInputSources).RaiseRefreshButtonPressed();
                                          //    return true; // handled
                                          // }
                                          return false; // not handled
                                       };

      return (appEventSources, appEventSinks, programInputSources);


      bool isQuitKey   (IKeyPressInfo keyPressInfo) => keyPressInfo.KeyData.KeyChar == 'Q';
   
      // bool isRefreshKey(IKeyPressInfo keyPressInfo) =>  keyPressInfo.KeyData.Key == ConsoleKey.F5. .HasFlag(Keys.F5)
      //                                                           && !keyPressInfo.KeyData.HasFlag(Keys.Modifiers);
   }


   // private static void quitApplication(Form mainForm) {
   //    mainForm.Close();
   // }


   // private static void displayNewView(MainForm form, View newView) {
   //    form.Controls.Clear();
   //    form.Controls.AddRange(newView.Controls);
   //    form.Invalidate();
   //    form.Controls[0].Focus();
   // }
}
