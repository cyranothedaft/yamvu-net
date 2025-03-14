using System;
using ConsoleSample.UIBasics;
using ConsoleSample.ViewPlatform;
using ConsoleSampleMvu.AppCore;
using Microsoft.Extensions.Logging;


namespace ConsoleSample;

internal static class EntryPoint {
   internal static async Task Main(string[] args) {
      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                                  .SetMinimumLevel(minimumLogLevel));

      ILogger  uiLogger = loggerFactory.CreateLogger("ui");
      ILogger inpLogger = loggerFactory.CreateLogger("inp");
      ILogger appLogger = loggerFactory.CreateLogger("app");
      ILogger svcLogger = loggerFactory.CreateLogger("svc");
      ILogger runLogger = loggerFactory.CreateLogger("run");
      ILogger prgLogger = loggerFactory.CreateLogger("prg");
      ILogger  fxLogger = loggerFactory.CreateLogger("fx");
      ILogger busLogger = loggerFactory.CreateLogger("bus");


      ProgramEventSources programInputSources = new(inpLogger);

      bool handleKeyPressAndRaiseProgramEvent(IKeyPressInfo keyPressed) {
         if (isQuitKey(keyPressed)) {
            ((IProgramEventSource_QuitButtonPressed)programInputSources).RaiseQuitButtonPressed();
            return true; // handled
         }

         // else if (isRefreshKey(args)) {
         //    ((IProgramEventSource_RefreshButtonPressed)programInputSources).RaiseRefreshButtonPressed();
         //    return true; // handled
         // }
         return false; // not handled

         bool isQuitKey(IKeyPressInfo keyPressInfo) => keyPressInfo.KeyData.IsEscape();
      }


      // establish the "view platform": essentially the state that is common to (and thus shared by) all views;
      //   this handles all user input and output (keyboard, display, etc.)
      using var viewPlatform = new ScrollingConsoleViewPlatform(handleKeyPressAndRaiseProgramEvent, inpLogger, uiLogger);

      AppMain<View> appMain = AppMain<View>.Build(programInputSources,
                                                  loggers: (appLogger, svcLogger, runLogger, prgLogger, fxLogger, busLogger));

      appLogger?.LogInformation("Started, running program to completion...");
      try {
         // start receiving input
         viewPlatform.StartMonitoringKeyboard();

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
