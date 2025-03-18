using System;
using ConsoleSample.UIBasics;
using ConsoleSample.View;
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

      MutableViewInputBindings programInputBindings = new(); // thread-safe table of delegates to call (which will be updated for each View) when keys are pressed (instead of going through event subscription/unsubscription for each View instance)
      programInputSources.QuitButtonPressed            += () => programInputBindings.QuitButtonPressed           ?.Invoke();
      programInputSources.Increment1ButtonPressed      += () => programInputBindings.Increment1ButtonPressed     ?.Invoke(); 
      programInputSources.IncrementRandomButtonPressed += () => programInputBindings.IncrementRandomButtonPressed?.Invoke();

      void updateBindings(ViewInputBindings newBindings) {
         programInputBindings.QuitButtonPressed            = newBindings.QuitButtonPressed;
         programInputBindings.Increment1ButtonPressed      = newBindings.Increment1ButtonPressed;
         programInputBindings.IncrementRandomButtonPressed = newBindings.IncrementRandomButtonPressed;
      }

      bool handleAppKeyPressAndRaiseProgramKeyEvent(IKeyPressInfo keyPressed)
         => ProgramKeyDispatcher.Handle(programInputSources, keyPressed);

      // establish the "view platform": essentially the state that is common to (and thus shared by) all views;
      //   this handles all user input and output (keyboard, display, etc.)
      using var viewPlatform = new ScrollingConsoleViewPlatform(handleAppKeyPressAndRaiseProgramKeyEvent, inpLogger, uiLogger);

      AppMain<PlatformView<MvuView>> appMain = AppMain<PlatformView<MvuView>>.Build(programInputSources,
                                                        loggers: (appLogger, svcLogger, runLogger, prgLogger, fxLogger, busLogger));

      appLogger?.LogInformation("Started, running program to completion...");
      try {
         // start receiving input
         viewPlatform.StartMonitoringKeyboard();

         // emit initial view
         ViewRenderer.DisplayView(ViewBuilder.BuildInitialView(), updateBindings);

         // run the program until it terminates
         await appMain.RunProgramWithCommonBusAsync(replaceViewAction: platformView => ViewRenderer.DisplayView(platformView, updateBindings),
                                                    viewFunc: ViewBuilder.BuildViewFromModel);

         appLogger?.LogInformation("Program terminated normally - application will now end.");
      }
      catch (Exception exception) {
         // TODO ===
         await Console.Error.WriteLineAsync(exception.ToString());
      }
   }
}
