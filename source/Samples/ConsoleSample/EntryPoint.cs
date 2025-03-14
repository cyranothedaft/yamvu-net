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

      ILogger uiLogger = loggerFactory.CreateLogger("ui");
      ILogger appLogger = loggerFactory.CreateLogger("app");

      using InputWatcher inputWatcher = new();

      // multiple regimes of abstraction for user/application input events
      // - "Application" events
      //    - general events from input devices, such as key-press events
      //    - the handler for these events should translate them to program-level events, then raise them in-turn
      // - "Program" events
      //    - specific events that are meaningful to the program itself, such as:
      //      - QuitKeyPressed      (user pressed the Esc key)
      //      - RefreshKeyPressed   (user pressed F5 to refresh the view)

      // define MVU program and wire up message loop
      ( _,
        AppEventSinks appEventSinks,            // <-- handlers for "Application" events; has been wired up to raise "Program" events (via the program event sources).
                                                //     Important! - This still needs to be wired up to the emitter of the actual "Platform" events (inputWatcher, in this case)
        ProgramEventSources programInputSources // <-- sources which raise "Program" events, which will be passed to the MVU program to be handled there
      ) = wireUpEvents(uiLogger);
      inputWatcher.SetHandler(appEventSinks);

      AppMain<View> appMain = AppMain<View>.Build(programInputSources,
                                                  loggers: (appLogger,
                                                            loggerFactory.CreateLogger("svc"),
                                                            loggerFactory.CreateLogger("run"),
                                                            loggerFactory.CreateLogger("prg"),
                                                            loggerFactory.CreateLogger("fx"),
                                                            loggerFactory.CreateLogger("bus"))
                                                 );

      appLogger?.LogInformation("Started, running program to completion...");
      try {
         // start receiving input
         inputWatcher.StartMonitoringKeyboard();

         // emit initial view
         ViewDisplayer.OutputView(ViewBuilder.BuildInitialView());

         // run the program until it terminates
         await appMain.RunProgramWithCommonBusAsync(replaceViewAction: ViewDisplayer.OutputView,
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


      bool isQuitKey(IKeyPressInfo keyPressInfo) => keyPressInfo.KeyData.IsEscape();
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
