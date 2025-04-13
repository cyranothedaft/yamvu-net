using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3CounterSample.View;
using yamvu;
using yamvu.core;
using yamvu.Runners;



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3CounterSample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application {

   private const LogLevel MinimumLogLevel = LogLevel.Trace;

   private MainWindow? _mainWindow;


   /// <summary>
   /// Initializes the singleton application object.  This is the first line of authored code
   /// executed, and as such is the logical equivalent of main() or WinMain().
   /// </summary>
   public App() {
      this.InitializeComponent();
   }


   /// <summary>
   /// Invoked when the application is launched.
   /// </summary>
   /// <param name="args">Details about the launch request and process.</param>
   protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
      _mainWindow = new MainWindow();
      embedMvuProgramInWindow(_mainWindow);
      _mainWindow.Activate();
   }


   private static void embedMvuProgramInWindow(MainWindow window) {
      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onActivatedRunMvuProgram(object sender, WindowActivatedEventArgs args) {
         try {
            // immediately unsubscribe so this only happens once
            window.Activated -= onActivatedRunMvuProgram;

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                             .SetMinimumLevel(MinimumLogLevel));
            //TODO ? _globalAppLogger = _loggerFactory?.CreateLogger("main");

            // windows is activated, so start (asynchronously run) the MVU program
            await runMvuProgramAsync(externalMessageDispatcher,
                                     replaceViewAction: view => replaceMvuComponents(window.MvuComponentContainer, view),
                                     loggerFactory);

            // the MVU program has terminated normally, so signal the window to close
            window.Close();
         }
         catch (Exception exception) {
            // TODO !!! _globalAppLogger?.LogError(exception, "General exception while running MVU program");
            // TODO: form.Close() ?
         }
      }

      void onClosedStopMvuProgram(object sender, WindowEventArgs args) {
         // window is closing, so signal the MVU program to terminate
         externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
      }

      window.Activated += onActivatedRunMvuProgram;
      window.Closed    += onClosedStopMvuProgram;
   }


   private static async Task runMvuProgramAsync(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<ProgramView>> replaceViewAction,
                                                ILoggerFactory? loggerFactory) {
      ILogger? servicesLogger = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");
      ILogger? programLogger = loggerFactory?.CreateLogger("prog");
      IAppServices appServices = new AppServices_Real(servicesLogger);

      PlatformView<ProgramView> view(MvuMessageDispatchDelegate dispatch, Model model)
         => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

      MvuProgramComponent<Model, PlatformView<ProgramView>> mvuComponent = Component.GetAsComponent(appServices, view, programLogger, loggerFactory);

      // var programRunnerWithServices = ProgramRunnerWithServices<PlatformView<ProgramView>>.Build(externalMessageDispatcher, loggerFactory);

      // run the program until it terminates
      var finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                               mvuComponent.BuildProgram,
                                                                               replaceViewAction,
                                                                               loggerFactory,
                                                                               externalMessageDispatcher,
                                                                               mvuComponent.ProgramInfo,
                                                                               mvuComponent.MessageAsCommandFunc,
                                                                               mvuComponent.ExecuteEffectDelegate,
                                                                               mvuComponent.IsQuitMessageFunc);
   }


   private static void replaceMvuComponents(Panel componentContainer, PlatformView<ProgramView> view) {
      componentContainer.Children.Clear();
      foreach (UIElement uiElement in view.MvuView.Controls) {
         componentContainer.Children.Add(uiElement);
      }
   }

}
