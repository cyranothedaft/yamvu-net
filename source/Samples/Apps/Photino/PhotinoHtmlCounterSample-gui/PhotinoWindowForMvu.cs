using System;
using System.Drawing;
using System.Threading.Tasks;
using CounterSample.AppCore.Mvu.Messages;
using Microsoft.Extensions.Logging;
using Photino.NET;
using yamvu;
using yamvu.Runners;


namespace PhotinoHtmlCounterSample.gui;

internal static class PhotinoWindowForMvu {
   internal static PhotinoWindow Build<TModel>(MvuProgramComponent<TModel, PhotinoView> mvuProgramComponent,
                                               ExternalMessageDispatcher externalMessageDispatcher,
                                               Action<Exception> handleException,
                                               ILogger? appLogger,
                                               ILoggerFactory? loggerFactory) {
      // Window title declared here for visibility
      string windowTitle = "Photino for .NET Demo App";

      // Creating a new PhotinoWindow instance with the fluent API
      var window = new PhotinoWindow().SetTitle(windowTitle)
                                       // Resize to a percentage of the main monitor work area
                                      .SetUseOsDefaultSize(false)
                                      .SetSize(new Size(1024, 800))
                                       // Center window in the middle of the screen
                                      .Center()
                                       // Users can resize windows by default.
                                       // Let's make this one fixed instead.
                                      .SetResizable(false)
                                      .SetContextMenuEnabled(false)
                                      .SetDevToolsEnabled(false)
            // .RegisterDynamicScript()
            // .RegisterWebMessageHandler()
            ;

      window.WindowClosing += (sender, args) => {
                                 appLogger?.LogTrace("# WindowClosing");
                                 externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
                                 return false; // let it close
                              };

      window.RegisterWebMessageReceivedHandler(handleWebMessage);

      return window;

      async void handleWebMessage(object? sender, string message) {
         try {
            appLogger?.LogTrace("### web message [{str}]", message);
            if (message == "StartMvuProgram") {
               await runMvuProgramAsync1();
            }
            else if (message.StartsWith("msg:")) {
               switch (message[4..]) {
                  case "increment1":      externalMessageDispatcher.Dispatch(MvuMessages.Request_Increment1()); break;
                  case "incrementrandom": externalMessageDispatcher.Dispatch(MvuMessages.Request_IncrementRandom()); break;
               }
            }
         }
         catch (Exception exception) {
            handleException(exception);
         }
      }

      async Task runMvuProgramAsync1() {
         appLogger?.LogTrace(">> MVU program");
         await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuProgramComponent.BuildProgramRunner, 
                                                                 mvuProgramComponent.BuildProgram, 
                                                                 view => updateView(window, view),
                                                                 loggerFactory, 
                                                                 externalMessageDispatcher, 
                                                                 mvuProgramComponent.ProgramInfo,
                                                                 mvuProgramComponent.MessageAsCommandFunc, 
                                                                 mvuProgramComponent.ExecuteEffectDelegate, 
                                                                 mvuProgramComponent.IsQuitMessageFunc);
               
         appLogger?.LogTrace("<< MVU program");

         // the MVU program has ended, so signal the window to close (if it's not closed already)
         appLogger?.LogTrace(">> Closing window");
         window.Close();
         appLogger?.LogTrace("<< Closing window");
      }

      // attachMvuProgramToWindow(window,
      //                          mvuProgramComponent.ProgramRunner,
      //                          mvuProgramComponent.Program,
      //                          mvuProgramComponent.ProgramInfo,
      //                          mvuProgramComponent.MessageAsCommandFunc,
      //                          mvuProgramComponent.IsQuitMessageFunc,
      //                          mvuProgramComponent.ExecuteEffectDelegate,
      //                          externalMessageDispatcher,
      //                          handleException, appLogger, loggerFactory);

   }


   private static void updateView(PhotinoWindow window, PhotinoView view) {
      window.SendWebMessage(view.HtmlFragment);
   }


   // private static void attachMvuProgramToWindow<TModel>(PhotinoWindow window,
   //                                                      IMvuProgramRunner<PhotinoView> mvuProgramRunner,
   //                                                      IMvuProgram2<TModel, PhotinoView> mvuProgram,
   //                                                      ProgramInfo programInfo,
   //                                                      Func<IMvuMessage, IMvuCommand> messageAsCommandFunc, 
   //                                                      Func<IMvuMessage, bool> isQuitMessageFunc,
   //                                                      ExecuteEffectDelegate<IMvuEffect> executeEffect,
   //                                                      ExternalMessageDispatcher externalMessageDispatcher,
   //                                                      Action<Exception> handleException,
   //                                                      ILogger? appLogger,
   //                                                      ILoggerFactory? loggerFactory) {
   //    window.WindowClosing += (sender, args) => {
   //                               appLogger?.LogTrace("# WindowClosing");
   //                               externalMessageDispatcher.Dispatch(MvuMessages.Request_Quit());
   //                               return false; // let it close
   //                            };
   //
   //    window.RegisterWebMessageReceivedHandler(handleWebMessage);
   //
   //    return;
   //
   //    async void handleWebMessage(object? sender, string message) {
   //       try {
   //          appLogger?.LogTrace("### web message [{str}]", message);
   //          if (message == "StartMvuProgram") {
   //             await runMvuProgramAsync1();
   //          }
   //          else if (message.StartsWith("msg:")) {
   //             switch (message[4..]) {
   //                case "increment1":      externalMessageDispatcher.Dispatch(MvuMessages.Request_Increment1()); break;
   //                case "incrementrandom": externalMessageDispatcher.Dispatch(MvuMessages.Request_IncrementRandom()); break;
   //             }
   //          }
   //       }
   //       catch (Exception exception) {
   //          handleException(exception);
   //       }
   //    }
   //
   //    async Task runMvuProgramAsync1() {
   //       appLogger?.LogTrace(">> MVU program");
   //       await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuProgramRunner, mvuProgram, view => updateView(window, view),
   //                                                               loggerFactory, externalMessageDispatcher, programInfo,
   //                                                               messageAsCommandFunc, executeEffect, isQuitMessageFunc);
   //
   //             
   //       appLogger?.LogTrace("<< MVU program");
   //
   //       // the MVU program has ended, so signal the window to close (if it's not closed already)
   //       appLogger?.LogTrace(">> Closing window");
   //       window.Close();
   //       appLogger?.LogTrace("<< Closing window");
   //    }
   // }
}
