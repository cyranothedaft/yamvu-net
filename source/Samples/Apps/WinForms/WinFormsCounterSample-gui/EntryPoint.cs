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
         ILogger? svcsLogger = _loggerFactory?.CreateLogger("svcs");
         ILogger? uiLogger   = _loggerFactory?.CreateLogger("ui");

         IAppServices appServices = new AppServices_Real(svcsLogger);

         PlatformView<ProgramView> view(MvuMessageDispatchDelegate dispatch, Model model)
            => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

         MainForm mainForm = new MainForm();
         WinFormsMvuHost.RunApp_SynchronousBlocking(mainForm,
                                                    MvuMessages.Request_Quit, 
                                                    () => Component.GetAsComponent(appServices, view, _loggerFactory?.CreateLogger("prog"), _loggerFactory),
                                                    _loggerFactory);

         // WinFormsMvuHost winFormsMvuHost  = new WinFormsMvuHost();
         // winFormsMvuHost  .EmbedMvuProgramInForm(mainForm, TODO);
         // winFormsMvuHost  .RunApp_SynchronousBlocking(mainForm);
      }
   }




}
