using System;
using System.Threading.Tasks;
using CounterSample.AppCore;
using CounterSample.AppCore.Mvu;
using CounterSample.AppCore.Services;
using Microsoft.Extensions.Logging;
using yamvu;
using yamvu.core;


namespace PhotinoHtmlCounterSample.gui;

//NOTE: To hide the console window, go to the project properties and change the Output Type to Windows Application.
// Or edit the .csproj file and change the <OutputType> tag from "WinExe" to "Exe".
internal static class EntryPoint {
   [STAThread]
   static void Main(string[] args) {
      using ILoggerFactory loggerFactory = buildLoggerFactory(LogLevel.Trace);

      ILogger appLogger = loggerFactory.CreateLogger("app");
      ILogger? servicesLogger = loggerFactory?.CreateLogger("svcs");
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");
      ILogger? programLogger = loggerFactory?.CreateLogger("prog");

      AppServices_Real appServices = new AppServices_Real(servicesLogger);

      PhotinoView view(MvuMessageDispatchDelegate dispatch, Model model)
         => ViewBuilder.BuildView(dispatch, model, uiLogger);

      void handleException(Exception exception) {
         appLogger?.LogError(exception, "Application error");
      }

      ExternalMessageDispatcher externalMessageDispatcher = new();
      MvuProgramComponent<Model, PhotinoView> mvuComponent = Component.GetAsComponent(appServices, view, programLogger, loggerFactory);
      PhotinoWindowForMvu.Build(mvuComponent,
                                externalMessageDispatcher,
                                handleException,
                                appLogger,
                                loggerFactory)
                         .Load("wwwroot/index.html") // Can be used with relative path strings or "new URI()" instance to load a website.
                         .WaitForClose();            // Starts the application event loop
   }


   private static ILoggerFactory buildLoggerFactory(LogLevel minimumLogLevel)
      => LoggerFactory.Create(builder =>
                                    builder.AddDebug()
                                           .SetMinimumLevel(minimumLogLevel));

}