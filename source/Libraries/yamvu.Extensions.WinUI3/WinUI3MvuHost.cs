using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using yamvu.core.Primitives;
using yamvu.Runners;



namespace yamvu.Extensions.WinUI3;

public static class WinUI3MvuHost {
   public static void StartMvuProgramInWindow<TWin, TModel, TView>(this TWin window,
                                                                   Func<IMvuMessage> getQuitMessage,
                                                                   Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                                   ILoggerFactory? loggerFactory
   ) where TWin : Window, IMvuComponentContainer
     where TView : IWinUI3View {
      embedMvuProgramInWindow(window, buildMvuComponent, getQuitMessage, loggerFactory);
      window.Activate();
   }


   private static void embedMvuProgramInWindow<TWin, TModel, TView>(TWin window,
                                                                    Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                                    Func<IMvuMessage> getQuitMessage,
                                                                    ILoggerFactory? loggerFactory)
         where TWin : Window, IMvuComponentContainer
         where TView : IWinUI3View {

      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onActivatedRunMvuProgram(object sender, WindowActivatedEventArgs args) {
         try {
            // immediately unsubscribe so this only happens once
            window.Activated -= onActivatedRunMvuProgram;

            MvuProgramComponent<TModel, TView> mvuComponent = buildMvuComponent();

            // windows is activated, so start (asynchronously run) the MVU program
            await runMvuProgramAsync(mvuComponent,
                                     externalMessageDispatcher,
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
         externalMessageDispatcher.Dispatch(getQuitMessage());
      }

      window.Activated += onActivatedRunMvuProgram;
      window.Closed    += onClosedStopMvuProgram;
   }


   private static async Task runMvuProgramAsync<TModel, TView>(MvuProgramComponent<TModel, TView> mvuComponent,
                                                               ExternalMessageDispatcher? externalMessageDispatcher, Action<TView> replaceViewAction,
                                                               ILoggerFactory? loggerFactory) {
      // run the program until it terminates
      TModel finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                                  mvuComponent.BuildProgram,
                                                                                  replaceViewAction,
                                                                                  loggerFactory,
                                                                                  externalMessageDispatcher,
                                                                                  mvuComponent.ProgramInfo,
                                                                                  mvuComponent.MessageAsCommandFunc,
                                                                                  mvuComponent.ExecuteEffectDelegate,
                                                                                  mvuComponent.IsQuitMessageFunc);
   }


   private static void replaceMvuComponents<TView>(Panel componentContainer, TView view) where TView : IWinUI3View {
      componentContainer.Children.Clear();
      foreach (UIElement uiElement in view.Contents) {
         componentContainer.Children.Add(uiElement);
      }
   }
}
