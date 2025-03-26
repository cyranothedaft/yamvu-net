using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CounterMvu_lib.Messages;
using CounterSample.AppCore;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using yamvu;



namespace WinFormsCounterSample.gui;

internal class MvuPlatformProgram {
   private readonly ExternalMessageDispatcher? _messageFromOutsideDispatcher;

   private bool _wasExitAlreadyRequested = false;

   public event Action<ProgramView>? ViewEmitted;
   public event Action? ProgramExited;


   public MvuPlatformProgram(ExternalMessageDispatcher? messageFromOutsideDispatcher) {
      _messageFromOutsideDispatcher = messageFromOutsideDispatcher;
   }


   public async Task RunAsync() {
      ProgramRunnerWithServices<PlatformView<ProgramView>> programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build( //programInputSources,
                                                                                                                                                  _messageFromOutsideDispatcher,
                                                                                                                                                  loggers: (null, null, null, null, null, null));
      
      // run the program until it terminates
      await programRunnerWithServices.RunProgramWithCommonBusAsync(replaceViewAction: platformView => {
                                                                                         ViewEmitted?.Invoke(platformView.MvuView);
                                                                                      },
                                                                   viewFunc: ViewBuilder.BuildViewFromModel
                                                                  );
      ProgramExited?.Invoke();
   }


   public void ExternalExitRequested(object? sender, EventArgs eventArgs) {
      if ( !_wasExitAlreadyRequested ) {
         _wasExitAlreadyRequested = true;
         _messageFromOutsideDispatcher?.Dispatch(MvuMessages.Request_Quit());
      }
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
