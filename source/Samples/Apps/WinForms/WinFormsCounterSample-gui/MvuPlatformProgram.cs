using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CounterSample.AppCore;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;



namespace WinFormsCounterSample.gui;

internal class MvuPlatformProgram {
   private Control _componentContainer;


   public event Action<ProgramView>? ViewEmitted;


   // public void SetComponentContainer(Control componentContainer) {
   //    _componentContainer = componentContainer;
   //
   //    // form.Shown += startMvuProgram;
   //    // TODO: anything?
   // }


   public async Task StartAsync() {
      ProgramRunnerWithServices<PlatformView<ProgramView>> programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build( //programInputSources,
                                                                                                                                                  loggers: (null, null, null, null, null, null));
      
      // run the program until it terminates
      await programRunnerWithServices.RunProgramWithCommonBusAsync(replaceViewAction: platformView => {
                                                                                         ViewEmitted?.Invoke(platformView.MvuView);
                                                                                      },
                                                                   viewFunc: ViewBuilder.BuildViewFromModel);
   }


   public void ExternalExitRequested() {
      signalQuitRequest
   }


   // private static void run(Action<Form> runApplication) {
   //    const LogLevel minimumLogLevel = LogLevel.Trace;
   //    using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
   //                                                                                .SetMinimumLevel(minimumLogLevel));
   //    ILogger? baseLogger = loggerFactory.CreateLogger("");
   //
   //    ILogger  uiLogger = loggerFactory.CreateLogger("ui");
   //    ILogger inpLogger = loggerFactory.CreateLogger("inp");
   //    ILogger appLogger = loggerFactory.CreateLogger("app");
   //    ILogger svcLogger = loggerFactory.CreateLogger("svc");
   //    ILogger runLogger = loggerFactory.CreateLogger("run");
   //    ILogger prgLogger = loggerFactory.CreateLogger("prg");
   //    ILogger  fxLogger = loggerFactory.CreateLogger("fx");
   //    ILogger busLogger = loggerFactory.CreateLogger("bus");
   //    ILogger plfLogger = loggerFactory.CreateLogger("plf");
   //
   //
   //    // ProgramEventSources programInputSources = new(inpLogger);
   //
   //    MutableInputBindingTable programInputBindings = new(); // thread-safe table of delegates to call (which will be updated for each View) when consequential events occur/originate outside the view itself
   //    // programInputSources.QuitButtonPressed            += () => programInputBindings.QuitButtonPressed           ?.Invoke();
   //
   //    void updateBindings(MutableInputBindingTable newBindings) {
   //       programInputBindings.QuitButtonPressed            = newBindings.QuitButtonPressed;
   //    }
   //
   //    MainForm mainForm = new();
   //
   //    // using var viewPlatform = new WinFormsViewPlatform(mainForm, inpLogger, uiLogger);
   //
   //    ProgramRunnerWithServices<PlatformView<ProgramView>> programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build(//programInputSources,
   //                                                                                  loggers: (appLogger, svcLogger, runLogger, prgLogger, fxLogger, busLogger));
   //
   //    runMvuProgram(programRunnerWithServices);
   //
   //       baseLogger?.LogTrace("application running with main form");
   //       runApplication(mainForm);
   //       baseLogger?.LogTrace("application ended");
   //
   //       appLogger?.LogInformation("Program terminated normally - application will now end.");
   // }


}
