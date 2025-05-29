using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using yamvu.core.Primitives;
using yamvu.Extensions.WebView.Library.WebView;



namespace yamvu.Extensions.WebView;

public delegate Task HandleMessageFromWebViewAsyncDelegate(string webMessageReceived, Func<string, Task> executeScriptAsyncAction, Action<IMvuMessage> dispatchMessage);

public static class WebViewMvuHost {



   public static void EmbedMvuProgram<TModel, TView>(this MinimalWebView webView,
                                                     Func<IMvuMessage> getQuitMessage, Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                     ILoggerFactory? loggerFactory) {
      ILogger? appLogger = loggerFactory?.CreateLogger("app");

      ExternalMessageDispatcher externalMessageDispatcher = new();


      webView.MessageFromPage += message => handleWebMessageAsync(message,
                                                                  exception => throw exception);

      async void handleWebMessageAsync(string webMessage, Action<Exception> handleException) {
         try {
            Console.WriteLine("===[ {0} ]===", webMessage);
            await webView.ExecuteScriptAsync($"alert('Hi from the UI thread! I got a message from the browser: {webMessage}')");
         }
         catch (Exception exception) {
            handleException(exception);
         }
      }


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
}
