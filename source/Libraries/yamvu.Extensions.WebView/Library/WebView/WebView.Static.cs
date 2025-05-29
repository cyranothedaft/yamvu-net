using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Extensions.Logging;
using yamvu.Extensions.WebView.Library.Window;



namespace yamvu.Extensions.WebView.Library.WebView;

partial class MinimalWebView {
   private const string StaticFileDirectory = "wwwroot";
   private const string VirtualHostName = "minimalwebview.example";


   public static MinimalWebView Init(MinimalWindow window, ILogger? logger) {
      MinimalWebView webView = new(logger);
      webView.initController(window.Handle);
      return webView;
   }

   
   private static async Task initControllerAsync(HWND hwnd, HandleMessageFromWebViewDelegate handleMessageFromWebView, ILogger? logger,
                                                 Action<CoreWebView2Controller> setControllerAction) {
      CoreWebView2Controller webViewController = await initControllerActualAsync();
      setControllerAction(webViewController);


      async Task<CoreWebView2Controller> initControllerActualAsync() {
         CoreWebView2Controller controller = await createCoreWebView2Async(hwnd, logger);
         setupWebView(controller, hwnd, handleMessageFromWebView);
         navigateWebView(controller);
         return controller;
      }

      static void setupWebView(CoreWebView2Controller controller, HWND hwnd, HandleMessageFromWebViewDelegate handleMessageFromWebView) {
         controller.DefaultBackgroundColor          =  Color.Transparent; // avoids flash of white when page first renders
         controller.CoreWebView2.WebMessageReceived += (sender, args) => CoreWebView2_WebMessageReceived(controller, sender, args);
         controller.CoreWebView2.SetVirtualHostNameToFolderMapping(VirtualHostName, StaticFileDirectory, CoreWebView2HostResourceAccessKind.Allow);
         PInvoke.GetClientRect(hwnd, out var hwndRect);
         controller.Bounds    = new Rectangle(0, 0, hwndRect.right, hwndRect.bottom);
         controller.IsVisible = true;

         void CoreWebView2_WebMessageReceived(CoreWebView2Controller controller, object? sender, CoreWebView2WebMessageReceivedEventArgs e) {
            string? webMessage = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(webMessage))
               return;

            handleMessageFromWebView(webMessage);
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

         Environment.Exit(1); // TODO: !?
      }
      catch (Exception ex) {
         PInvoke.MessageBox(hwnd, $"Failed to initialize WebView2:{Environment.NewLine}{ex}", "Error", MESSAGEBOX_STYLE.MB_OK | MESSAGEBOX_STYLE.MB_ICONERROR);
         Environment.Exit(1); // TODO: !?
      }
      throw new Exception("never here");
   }
}
