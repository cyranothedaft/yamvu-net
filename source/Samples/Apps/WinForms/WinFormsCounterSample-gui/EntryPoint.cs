using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CounterMvu_lib.Messages;
using CounterSample.AppCore;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using yamvu;



namespace WinFormsCounterSample.gui;

internal static class EntryPoint {
   private static ILoggerFactory? _loggerFactory = null;
   private static ILogger? _globalAppLogger = null;


   /// <summary>
   ///  The main entry point for the application.
   /// </summary>
   [STAThread]
   static void Main() {
      ApplicationConfiguration.Initialize();

      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ( _loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                      .SetMinimumLevel(minimumLogLevel))) {
         _globalAppLogger = _loggerFactory?.CreateLogger("main");
         MainForm mainForm = new MainForm();

         embedMvuProgramInForm(mainForm);
         Application.Run(mainForm);
      }
   }


   private static void embedMvuProgramInForm(MainForm form) {
      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onShownRunMvuProgram(object? sender, EventArgs e) {
         try {
            // form is loading - start (asynchronously run) the MVU program
            await runMvuProgramAsync(externalMessageDispatcher,
                                     viewEmittedCallback: view => replaceMvuComponents(form.MvuComponentContainer, view));

            // the MVU program has terminated normally - signal the form to close
            form.Close();
         }
         catch (Exception exception) {
            _globalAppLogger?.LogError(exception, "General exception while running MVU program");
            // TODO: form.Close() ?
         }
      }

      void onClosingStopMvuProgram(object? sender, FormClosingEventArgs e) {
         // form is closing - signal the MVU program to terminate
         externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
      }

      form.Load        += onShownRunMvuProgram;
      form.FormClosing += onClosingStopMvuProgram;
   }


   private static async Task runMvuProgramAsync(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<ProgramView>> viewEmittedCallback) {
      var programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build(externalMessageDispatcher, _loggerFactory);

      // run the program until it terminates
      await programRunnerWithServices.RunProgramWithCommonBusAsync(replaceViewAction: viewEmittedCallback,
                                                                   viewFunc: ViewBuilder.BuildViewFromModel,
                                                                   _loggerFactory);
   }


   private static void replaceMvuComponents(Control componentContainer, PlatformView<ProgramView> view) {
      componentContainer.SuspendLayout();
      componentContainer.Controls.Clear();
      componentContainer.Controls.AddRange(view.MvuView.Controls.ToArray());
      componentContainer.ResumeLayout();
      componentContainer.Invalidate();
   }


   private static void Program_ExitRequested(object? sender, EventArgs e) {
      // TODO?
   }


   private static async void RunAndHandleExceptionsAsync(Task task) {
      try {
         // Force this to yield to the caller, so Application.Run will be executing
         await Task.Yield();
         await task;
      }
      catch (Exception ex) {
         throw;
         //=== ...log the exception, show an error to the user, etc.
         Application.Exit();
      }
   }
}
