using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using yamvu.core.Primitives;
using yamvu.Extensions.WebView.Library.WebView;
using yamvu.Runners;



namespace yamvu.Extensions.WebView;

public delegate Task HandleMessageFromWebViewAsyncDelegate(string webMessageReceived, Func<string, Task> executeScriptAsyncAction, Action<IMvuMessage> dispatchMessage);

public static class WebViewMvuHost {



   public static async Task RunMvuProgramAsync<TModel, TView>(this MinimalWebView webView,
                                                     Func<IMvuMessage> getQuitMessage, 
                                                     Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                     Func<string,IMvuMessage> deserializeMessage,
                                                     ILoggerFactory? loggerFactory) 
   where TView:IWebViewView
   {
      ILogger? appLogger = loggerFactory?.CreateLogger("app");

      ExternalMessageDispatcher externalMessageDispatcher = new();


      webView.MessageFromPage += message => handleWebMessageAsync(message,
                                                                  exception => throw exception);

      async void handleWebMessageAsync(string webMessage, Action<Exception> handleException) {
         try {
            IMvuMessage message = deserializeMessage(webMessage);
            externalMessageDispatcher.Dispatch(message);
         }
         catch (Exception exception) {
            handleException(exception);
         }
      }

      // form has loaded, so start (asynchronously run) the MVU program
      await runMvuProgramAsync(externalMessageDispatcher,
                               replaceViewAction: view => updateView(webView, view),
                               buildMvuComponent,
                               loggerFactory);

//===TODO      // the MVU program has terminated normally, so signal the form to close
//===TODO      hostForm.Close();



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


   private static void updateView<TView>(MinimalWebView webView, TView view) where TView : IWebViewView {
      // TODO: find a way to send a message directly, thus avoiding the need to encode this
      string javascriptEncodedHtml = System.Text.Encodings.Web.JavaScriptEncoder.Default.Encode(view.Html);

      // fire and forget
      _ = webView.ExecuteScriptAsync("replaceView(" + javascriptEncodedHtml + ")");
   }
}
