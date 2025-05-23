using System;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Mvu.Messages;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using yamvu;
using yamvu.Extensions.WinForms;


namespace WinFormsCounterSample.gui;

internal static class EntryPoint {
   /// <summary>
   ///  The main entry point for the application.
   /// </summary>
   [STAThread]
   static void Main() {
      ApplicationConfiguration.Initialize();

      const LogLevel minimumLogLevel = LogLevel.Trace;
      using ( ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddDebug()
                                                                                    .SetMinimumLevel(minimumLogLevel)) ) {
         ILogger? svcsLogger = loggerFactory?.CreateLogger("svcs");
         ILogger? uiLogger = loggerFactory?.CreateLogger("ui");

         new MainForm()
              .RunMvuApp(() => MvuMessages.Request_Quit(),
                         // ReSharper disable once AccessToDisposedClosure
                         () => getComponent(svcsLogger, uiLogger, loggerFactory),
                         loggerFactory);
      }
   }


   private static MvuProgramComponent<Model, ProgramView> getComponent(ILogger? svcsLogger, ILogger? uiLogger, ILoggerFactory? loggerFactory)
      => Component.GetAsComponent(new AppServices_Real(svcsLogger),
                                  viewFunc: (dispatch, model) => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger),
                                  loggerFactory?.CreateLogger("prog"),
                                  loggerFactory);
}
