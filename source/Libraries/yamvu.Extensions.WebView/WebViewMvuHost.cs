using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using yamvu.core.Primitives;



namespace yamvu.Extensions.WebView;

public delegate Task HandleMessageFromWebViewAsyncDelegate(string webMessageReceived, Func<string, Task> executeScriptAsyncAction, Action<IMvuMessage> dispatchMessage);

public static class WebViewMvuHost {



   public static void RunMvuApp<TModel, TView>(HandleMessageFromWebViewAsyncDelegate handleMessageFromWebViewAsyncDelegate,
         Func<IMvuMessage> getQuitMessage, Func<MvuProgramComponent<TModel, TView>> buildMvuComponent,
                                                      ILoggerFactory? loggerFactory)
         {
      ILogger? appLogger = loggerFactory?.CreateLogger("app");

      ExternalMessageDispatcher externalMessageDispatcher = new();

      webView.


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
}
