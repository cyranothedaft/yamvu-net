using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.gui.ViewPlatform;
using WinFormsCounterSample.View;
using yamvu;
using yamvu.core;
using yamvu.Runners;



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

         embedMvuProgramInContainer(mainForm);
         Application.Run(mainForm);
      }
   }


   private static void embedMvuProgramInContainer(IMvuControlContainer container) {
      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onLoadRunMvuProgram(object? sender, EventArgs e) {
         try {
            // form has loaded, so start (asynchronously run) the MVU program
            await runMvuProgramAsync(externalMessageDispatcher,
                                     replaceViewAction: view => replaceMvuComponents(container.MvuComponentContainer, view));

            // the MVU program has terminated normally, so signal the form to close
            container.Close();
         }
         catch (Exception exception) {
            _globalAppLogger?.LogError(exception, "General exception while running MVU program");
            // TODO: form.Close() ?
         }
      }

      void onClosingStopMvuProgram(object? sender, FormClosingEventArgs e) {
         // form is closing, so signal the MVU program to terminate
         externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
      }

      container.ContainerLoaded  += onLoadRunMvuProgram;
      container.ContainerClosing += onClosingStopMvuProgram;
   }


   private static async Task runMvuProgramAsync(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<ProgramView>> replaceViewAction) {
      ILogger? uiLogger = _loggerFactory?.CreateLogger("ui");
      ILogger? servicesLogger = _loggerFactory?.CreateLogger("svcs");
      IAppServices appServices = new AppServices_Real(servicesLogger);

      PlatformView<ProgramView> view(MvuMessageDispatchDelegate dispatch, Model model)
         => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

      MvuProgramComponent<Model, PlatformView<ProgramView>> mvuComponent = Component.GetAsComponent(appServices, view, _loggerFactory?.CreateLogger("prog"), _loggerFactory);
      // var programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build(externalMessageDispatcher, _loggerFactory);
      var finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                               mvuComponent.BuildProgram,
                                                                               replaceViewAction,
                                                                               _loggerFactory,
                                                                               externalMessageDispatcher,
                                                                               mvuComponent.ProgramInfo,
                                                                               mvuComponent.MessageAsCommandFunc,
                                                                               mvuComponent.ExecuteEffectDelegate,
                                                                               mvuComponent.IsQuitMessageFunc);
   }


   private static void replaceMvuComponents(Control componentContainer, PlatformView<ProgramView> view) {
      componentContainer.SuspendLayout();
      componentContainer.Controls.Clear();
      componentContainer.Controls.AddRange(view.MvuView.Controls.ToArray());
      componentContainer.ResumeLayout();
      componentContainer.Invalidate();
   }
}
