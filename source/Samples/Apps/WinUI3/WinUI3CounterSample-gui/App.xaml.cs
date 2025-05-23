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
using yamvu.Extensions.WinUI3;
using yamvu.Runners;



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3CounterSample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application {
   private const LogLevel MinimumLogLevel = LogLevel.Trace;


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

      using var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                       .SetMinimumLevel(MinimumLogLevel));

      ILogger? servicesLogger = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");
      ILogger? programLogger = loggerFactory?.CreateLogger("prog");

      IAppServices appServices = new AppServices_Real(servicesLogger);

      new MainWindow()
           .StartMvuProgramInWindow(() => MvuMessages.Request_Quit(),
                                    () => Component.GetAsComponent(appServices,
                                                                   (dispatch, model) => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger),
                                                                   programLogger,
                                                                   // ReSharper disable once AccessToDisposedClosure
                                                                   loggerFactory),
                                    loggerFactory);
   }
}
