using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Extensions.Logging;
using yamvu.Extensions.WebView;



namespace MinimalWebViewCounterSample;

public class WebView {
   private const string StaticFileDirectory = "wwwroot";
   private const string VirtualHostName = "minimalwebview.example";

   // public event EventHandler<MessageFromWebViewEventArgs> MessageFromWebViewEvent;
   // public class MessageFromWebViewEventArgs:EventArgs{}


   // HandleMessageFromWebViewAsyncDelegate handleMessageFromWebViewAsync

   internal static WebView InitController(HWND hwnd, HandleMessageFromWebViewAsyncDelegate handleMessageFromWebViewAsync, ILogger? logger) {
      // Start initializing WebView2 in a fire-and-forget manner. Errors will be handled in the initialization function
      _ = initControllerAsync();

      return new WebView();

      async Task<CoreWebView2Controller> initControllerAsync() {
         CoreWebView2Controller controller = await createCoreWebView2Async(hwnd,
                                                                      // newcontroller => { controller = newcontroller; }, 
                                                                      logger);
         setupWebView(controller, hwnd, handleMessageFromWebViewAsync);
         navigateWebView(controller);
         return controller;
      }

      static void setupWebView(CoreWebView2Controller controller, HWND hwnd, HandleMessageFromWebViewAsyncDelegate handleMessageFromWebViewAsync) {
         controller.DefaultBackgroundColor          =  Color.Transparent; // avoids flash of white when page first renders
         controller.CoreWebView2.WebMessageReceived += (sender, args) => CoreWebView2_WebMessageReceived(controller, sender, args);
         controller.CoreWebView2.SetVirtualHostNameToFolderMapping(VirtualHostName, StaticFileDirectory, CoreWebView2HostResourceAccessKind.Allow);
         PInvoke.GetClientRect(hwnd, out var hwndRect);
         controller.Bounds    = new Rectangle(0, 0, hwndRect.right, hwndRect.bottom);
         controller.IsVisible = true;

         async void CoreWebView2_WebMessageReceived(CoreWebView2Controller controller, object? sender, CoreWebView2WebMessageReceivedEventArgs e) {
            try {
               string? webMessage = e.TryGetWebMessageAsString();
               if (string.IsNullOrEmpty(webMessage))
                  return;

               // // simulate moving some slow operation to a background thread
               // await Task.Run(() => Thread.Sleep(200));

               // this will blow up if not run on the UI thread, so the SynchronizationContext needs to have been wired up correctly
               await handleMessageFromWebViewAsync(webMessage, scriptExecutorAsync);
               // await controller.CoreWebView2.ExecuteScriptAsync($"replaceView('New View: {webMessage}')");

               Task scriptExecutorAsync(string javascript)
                  => controller.CoreWebView2.ExecuteScriptAsync(javascript);
            }
            catch (Exception exception) {
               throw; // TODO handle exception
            }
         }
      }

      static void navigateWebView(CoreWebView2Controller controller) {
         controller.CoreWebView2.Navigate($"https://{VirtualHostName}/index.html");
      }
   }


   private static async Task<CoreWebView2Controller> createCoreWebView2Async(HWND hwnd, ILogger? logger) {
      try {
         logger?.LogDebug("Initializing WebView2");
         var environment = await CoreWebView2Environment.CreateAsync(null, null, null);
         CoreWebView2Controller controller = await environment.CreateCoreWebView2ControllerAsync(hwnd);

         logger?.LogDebug("WebView2 initialization succeeded.");
         return controller;
      }
      catch (WebView2RuntimeNotFoundException) {
         var result = PInvoke.MessageBox(hwnd, "WebView2 runtime not installed.\r\n" +
                                               "Download and install:\r\n"           +
                                               "https://developer.microsoft.com/en-us/microsoft-edge/webview2?form=MA13LH",
                                         "Error", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);

         if (result == MESSAGEBOX_RESULT.IDYES) {
            //TODO: show message: download WV2 bootstrapper from https://go.microsoft.com/fwlink/p/?LinkId=2124703 and run it
         }

         Environment.Exit(1);
      }
      catch (Exception ex) {
         PInvoke.MessageBox(hwnd, $"Failed to initialize WebView2:{Environment.NewLine}{ex}", "Error", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);
         Environment.Exit(1);
      }
      throw new Exception("never here");
   }



}
