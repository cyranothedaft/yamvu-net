using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using yamvu.core.Primitives;
using yamvu.Runners;



namespace yamvu.Extensions.WinForms;

public static class WinFormsMvuHost {
   public static void RunApp_SynchronousBlocking<TForm, TModel, TView>(TForm hostForm, Func<IMvuMessage> getQuitMessage, Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                                       ILoggerFactory? loggerFactory)
         where TForm : Form, IMvuControlContainer
         where TView : IWinFormsView {
      embedMvuProgramInForm(hostForm, getQuitMessage, buildMvuComponent, loggerFactory);
      Application.Run(hostForm);
   }


   private static void embedMvuProgramInForm<TForm, TModel, TView>(TForm hostForm, Func<IMvuMessage> getQuitMessage, Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                                   ILoggerFactory? loggerFactory) where TForm : Form, IMvuControlContainer
                                                                                                  where TView : IWinFormsView {
      ILogger? appLogger = loggerFactory?.CreateLogger("app");

      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onLoadRunMvuProgram(object? sender, EventArgs e) {
         try {
            // form has loaded, so start (asynchronously run) the MVU program
            await runMvuProgramAsync(externalMessageDispatcher,
                                     replaceViewAction: view => replaceMvuComponents(hostForm.MvuComponentContainer, view),
                                     buildMvuComponent,
                                     loggerFactory);

            // the MVU program has terminated normally, so signal the form to close
            hostForm.Close();
         }
         catch (Exception exception) {
            appLogger?.LogError(exception, "General exception while running MVU program");

            // TODO: form.Close() ?
         }
      }

      void onClosingStopMvuProgram(object? sender, FormClosingEventArgs e) {
         // form is closing, so signal the MVU program to terminate
         externalMessageDispatcher.Dispatch(getQuitMessage());
      }

      hostForm.Load        += onLoadRunMvuProgram;
      hostForm.FormClosing += onClosingStopMvuProgram;
   }


   private static async Task runMvuProgramAsync<TModel, TView>(ExternalMessageDispatcher? externalMessageDispatcher, Action<TView> replaceViewAction,
                                                               Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                               ILoggerFactory? loggerFactory) {
      MvuProgramComponent<TModel, TView> mvuComponent = buildMvuComponent();
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


   private static void replaceMvuComponents<TView>(Control componentContainer, TView view) where TView : IWinFormsView {
      componentContainer.SuspendLayout();
      componentContainer.Controls.Clear();
      componentContainer.Controls.AddRange(view.Contents.ToArray());
      componentContainer.ResumeLayout();
      componentContainer.Invalidate();
   }
}
