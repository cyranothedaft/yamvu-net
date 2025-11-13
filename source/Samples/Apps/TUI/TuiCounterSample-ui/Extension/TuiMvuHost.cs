using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TuiCounterSample.ui;
using yamvu.core.Primitives;
using yamvu.Runners;


namespace yamvu.Extensions.Tui;

public static class TuiMvuHost {
   public static void RunMvuApp<TWin, TModel, TView>(this TWin mainView, Func<IMvuMessage> getQuitMessage, Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                     ILoggerFactory? loggerFactory)
         where TWin: Window, IMvuControlContainer
         where TView : ITuiView {
      embedMvuProgramInView(mainView, getQuitMessage, buildMvuComponent, loggerFactory);
      Application.Run(mainView);
      //           .Dispose();
   }


   private static void embedMvuProgramInView<TWin, TModel, TView>(TWin mainView, Func<IMvuMessage> getQuitMessage, Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                                  ILoggerFactory? loggerFactory)
         where TWin : Window, IMvuControlContainer
         where TView : ITuiView {
      ILogger? appLogger = loggerFactory?.CreateLogger("app");

      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onLoadRunMvuProgram(object? sender, EventArgs e) {
         try {
            // toplevel view has loaded, so start (asynchronously run) the MVU program
            var finalModel = await runMvuProgramAsync(externalMessageDispatcher,
                                                      replaceViewAction: view => replaceMvuComponents(mainView.MvuComponentContainer, view),
                                                      buildMvuComponent,
                                                      loggerFactory);
            // TODO: return finalModel somehow

            // the MVU program has terminated normally, so signal the toplevel view to close.
            mainView.RequestStop();
         }
         catch (Exception exception) {
            appLogger?.LogError(exception, "General exception while running MVU program");
            // TODO: form.Close() ?
         }
      }

      void onClosingStopMvuProgram(object? sender, ToplevelClosingEventArgs e) {
         // toplevel view is closing, so signal the MVU program to terminate
         externalMessageDispatcher.Dispatch(getQuitMessage());
      }

      mainView.Initialized += onLoadRunMvuProgram;
      mainView.Closing     += onClosingStopMvuProgram;
   }


   private static async Task<TModel> runMvuProgramAsync<TModel, TView>(ExternalMessageDispatcher? externalMessageDispatcher, Action<TView> replaceViewAction,
                                                                       Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                                       ILoggerFactory? loggerFactory) {
      MvuProgramComponent<TModel, TView> mvuComponent = buildMvuComponent();
      return await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                     mvuComponent.BuildProgram,
                                                                     replaceViewAction,
                                                                     loggerFactory,
                                                                     externalMessageDispatcher,
                                                                     mvuComponent.ProgramInfo,
                                                                     mvuComponent.MessageAsCommandFunc,
                                                                     mvuComponent.ExecuteEffectDelegate,
                                                                     mvuComponent.IsQuitMessageFunc);
   }


   private static void replaceMvuComponents<TView>(View containerView, TView programView) where TView : ITuiView {
      TuiHelper.ReplaceViewContents(containerView, programView.Contents);
   }
}
