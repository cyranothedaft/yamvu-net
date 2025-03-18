using System;
using System.Windows.Forms;
using CounterSample.AppCore;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;



namespace WinFormsCounterSample.gui;

internal static class EntryPoint {
   /// <summary>
   ///  The main entry point for the application.
   /// </summary>
   [STAThread]
   static void Main() {
      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();

      void runApplication(Form mainForm) {
         Application.Run(mainForm);
      }

      run(runApplication);
   }


   private static void run(Action<Form> runApplication) {
      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                                  .SetMinimumLevel(minimumLogLevel));
      ILogger? baseLogger = loggerFactory.CreateLogger("");

      MainForm mainForm = new();


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



      (AppEventSources appEventSources, AppEventSinks appEventSinks, ProgramEventSources programInputSources)
            = wireUpEvents(loggers.ui);
      wireUpMainFormKeyPressEvents(mainForm, appEventSinks);
      var appMain = AppMain<ProgramView>.Build(programInputSources, loggers);

      async void onShown(object? sender, EventArgs args) {
         await StartProgramAsync(mainForm, ProgramViewBuilder.BuildInitialView(), appMain, loggers.app);
      }

      mainForm.Shown += onShown;

      baseLogger?.LogTrace("application running with main form");
      runApplication(mainForm);
      baseLogger?.LogTrace("application ended");
   }
}
