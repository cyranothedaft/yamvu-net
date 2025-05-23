using System;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using yamvu;
using yamvu.core;
using yamvu.Extensions.WinForms;


namespace WinFormsCounterSample.gui;

internal static class EntryPoint {
   private static ILoggerFactory? _loggerFactory = null;


   /// <summary>
   ///  The main entry point for the application.
   /// </summary>
   [STAThread]
   static void Main() {
      ApplicationConfiguration.Initialize();

      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ( _loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                      .SetMinimumLevel(minimumLogLevel))) {
         ILogger? svcsLogger = _loggerFactory?.CreateLogger("svcs");
         ILogger? uiLogger   = _loggerFactory?.CreateLogger("ui");

         IAppServices appServices = new AppServices_Real(svcsLogger);

         ProgramView view(MvuMessageDispatchDelegate dispatch, Model model)
            => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

         MvuProgramComponent<Model, ProgramView> getComponent()
            => Component.GetAsComponent(appServices, view, _loggerFactory?.CreateLogger("prog"), _loggerFactory);

         MainForm mainForm = new MainForm();
         WinFormsMvuHost.RunApp_SynchronousBlocking(mainForm,
                                                    MvuMessages.Request_Quit,
                                                    getComponent,
                                                    _loggerFactory);
      }
   }
}
