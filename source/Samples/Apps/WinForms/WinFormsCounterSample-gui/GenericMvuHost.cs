using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using WinFormsCounterSample.gui.UI;
using WinFormsCounterSample.View;
using yamvu;
using yamvu.core;
using yamvu.core.Primitives;
using yamvu.Extensions.WinForms;
using yamvu.Runners;



namespace WinFormsCounterSample.gui;

public interface IMvuHost<TContext> {
   void RunApp_SynchronousBlocking(TContext context);
   void EmbedMvuProgramInForm(TContext context);
}



public class WinFormsMvuHost<TForm> : IMvuHost<TForm>
where TForm:Form,IMvuControlContainer {
   private readonly ILoggerFactory? _loggerFactory;


   public WinFormsMvuHost(ILoggerFactory? loggerFactory) {
      _loggerFactory = loggerFactory;
   }


   public void RunApp_SynchronousBlocking(TForm hostForm) {
      Application.Run(hostForm);
   }


   public void EmbedMvuProgramInForm(TForm hostForm) {
      embedMvuProgramInForm(hostForm, _loggerFactory);
   }


   private static void embedMvuProgramInForm(TForm hostForm, Func<IMvuMessage> getQuitMessage, ILoggerFactory? loggerFactory) {
      ILogger? appLogger = loggerFactory?.CreateLogger("app");

      ExternalMessageDispatcher externalMessageDispatcher = new();

      async void onLoadRunMvuProgram(object? sender, EventArgs e) {
         try {
            // form has loaded, so start (asynchronously run) the MVU program
            await runMvuProgramAsync(externalMessageDispatcher,
                                     replaceViewAction: view => replaceMvuComponents(hostForm.MvuComponentContainer, view),
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


   private static async Task runMvuProgramAsync<TModel, TView>(ExternalMessageDispatcher? externalMessageDispatcher, Action<PlatformView<TView>> replaceViewAction,
                                                               Func<MvuProgramComponent<TModel, PlatformView<TView>>> buildMvuComponent,
                                                               ILoggerFactory? loggerFactory) {
      ILogger? uiLogger = loggerFactory?.CreateLogger("ui");

      PlatformView<TView> view(MvuMessageDispatchDelegate dispatch, TModel model)
         => ViewBuilder.BuildViewFromModel(dispatch, model, uiLogger);

      MvuProgramComponent<TModel, PlatformView<TView>> mvuComponent = buildMvuComponent();
      // var programRunnerWithServices = ProgramRunnerWithServices<PlatformView<TView>>.Build(externalMessageDispatcher, _loggerFactory);
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


   private static void replaceMvuComponents<TView>(Control componentContainer, PlatformView<TView> view) {
      componentContainer.SuspendLayout();
      componentContainer.Controls.Clear();
      componentContainer.Controls.AddRange(view.MvuView.Controls.ToArray());
      componentContainer.ResumeLayout();
      componentContainer.Invalidate();
   }
}
