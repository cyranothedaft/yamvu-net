using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using yamvu.core.Primitives;
using yamvu.Runners;


namespace yamvu.Extensions.WebView;

public delegate Task HandleMessageFromWebViewAsyncDelegate(string webMessageReceived, Func<string, Task> executeScriptAsyncAction, Action<IMvuMessage> dispatchMessage);



public static class WebViewMvuHost {

   public static void AttachMvuProgram<TModel, TView>(this WebViewWindow webViewWindow,
                                                      Func<IMvuMessage> getQuitMessage,
                                                      Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                      Func<string, IMvuMessage> deserializeMessage,
                                                      ILogger? appLogger, ILoggerFactory? loggerFactory) where TView : IWebViewView {

      webViewWindow.WebView.Initialized += () => webViewWindow.WebView.LoadPage();
      webViewWindow.WebView.PageLoaded += runMvuProgram;
      return;


      async void runMvuProgram() {
         // WebView has loaded, so start (asynchronously run) the MVU program
         try {
            appLogger?.LogTrace("-->>-- WebViewLoaded");
            appLogger?.LogDebug("WebView loaded. Will now prepare MVU program.");
            await runMvuProgramAsync(webViewWindow, getQuitMessage, buildMvuComponent, deserializeMessage, appLogger, loggerFactory);
         }
         catch (Exception ex) {
            handleException(ex);
         }
      }

      static void handleException(Exception exception)
         => throw exception;
   }


   private static async Task runMvuProgramAsync<TModel, TView>(this WebViewWindow webViewWindow,
                                                               Func<IMvuMessage> getQuitMessage,
                                                               Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                               Func<string, IMvuMessage> deserializeMessage,
                                                               ILogger? appLogger,
                                                               ILoggerFactory? loggerFactory) where TView : IWebViewView {
      // setup

      appLogger?.LogDebug("MVU program - Setting up ");

      ExternalMessageDispatcher externalMessageDispatcher = new();


      webViewWindow.WebView.MessageFromPage += handleWebMessageAsync;
      webViewWindow.Window.Closing += () => {
                                         appLogger?.LogTrace("-->>-- WindowClosing");
                                         externalMessageDispatcher.Dispatch(getQuitMessage());
                                      };


      async void handleWebMessageAsync(string webMessage) {
         try {
            appLogger?.LogTrace("-->>-- MessageFromWebView: {webMessage}", webMessage);
            IMvuMessage message = deserializeMessage(webMessage);
            externalMessageDispatcher.Dispatch(message);
         }
         catch (Exception exception) {
            handleException(exception);
         }
      }

      static void handleException(Exception exception)
         => throw exception;

      // run

      appLogger?.LogDebug("MVU program - Starting");
      await runMvuProgramAsync(externalMessageDispatcher,
                               updateViewAction: view => {
                                                    appLogger?.LogTrace("-->>-- updateViewAction: {webMessage}", view);
                                                    updateView(webViewWindow, view);
                                                 },
                               buildMvuComponent,
                               loggerFactory);

      // the MVU program has terminated normally, so signal the window to close
      appLogger?.LogDebug("MVU program - Terminated normally. Will now close window.");
      webViewWindow.Window.Close();



      // async void onLoadRunMvuProgram(object? sender, EventArgs e) {
      //    try {
      //       // form has loaded, so start (asynchronously run) the MVU program
      //       await runMvuProgramAsync(externalMessageDispatcher,
      //                                replaceViewAction: view => replaceMvuComponents(hostForm.MvuComponentContainer, view),
      //                                buildMvuComponent,
      //                                loggerFactory);
      //
      //       // the MVU program has terminated normally, so signal the form to close
      //       hostForm.Close();
      //    }
      //    catch (Exception exception) {
      //       appLogger?.LogError(exception, "General exception while running MVU program");
      //
      //       // TODO: form.Close() ?
      //    }
      // }
      //
      // void onClosingStopMvuProgram(object? sender, FormClosingEventArgs e) {
      //    // form is closing, so signal the MVU program to terminate
      //    externalMessageDispatcher.Dispatch(getQuitMessage());
      // }
      //
      // hostForm.Load        += onLoadRunMvuProgram;
      // hostForm.FormClosing += onClosingStopMvuProgram;
   }


   private static async Task runMvuProgramAsync<TModel, TView>(ExternalMessageDispatcher? externalMessageDispatcher, Action<TView> updateViewAction,
                                                               Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                               ILoggerFactory? loggerFactory) {
      MvuProgramComponent<TModel, TView> mvuComponent = buildMvuComponent();
      var finalModel = await ProgramRunnerWithBus.RunProgramWithCommonBusAsync(mvuComponent.BuildProgramRunner,
                                                                               mvuComponent.BuildProgram,
                                                                               updateViewAction,
                                                                               loggerFactory,
                                                                               externalMessageDispatcher,
                                                                               mvuComponent.ProgramInfo,
                                                                               mvuComponent.MessageAsCommandFunc,
                                                                               mvuComponent.ExecuteEffectDelegate,
                                                                               mvuComponent.IsQuitMessageFunc);
   }


   private static void updateView<TView>(WebViewWindow webViewWindow, TView view) where TView : IWebViewView {
      // TODO: find a way to send a message directly, thus avoiding the need to encode this
      string javascriptEncodedHtml = encodeViewContents(view.Html);
      //string javascriptEncodedHtml = System.Text.Encodings.Web.JavaScriptEncoder.Default.Encode(view.Html);

      // fire and forget
      _ = webViewWindow.WebView.ExecuteScriptAsync($"window.replaceHtml(atob('{javascriptEncodedHtml}'))")
                               .ConfigureAwait(continueOnCapturedContext: false);
   }


   private static string encodeViewContents(string view)
      => Convert.ToBase64String(Encoding.UTF8.GetBytes(view));
}
