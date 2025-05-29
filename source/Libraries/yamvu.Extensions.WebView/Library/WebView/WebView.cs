using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;


namespace yamvu.Extensions.WebView.Library.WebView;

public partial class MinimalWebView {
   public delegate void HandleMessageFromWebViewDelegate(string webMessageReceived);


   private readonly ILogger? _logger;
   private CoreWebView2Controller? _controller;


   public event HandleMessageFromWebViewDelegate? MessageFromPage;


   private MinimalWebView(ILogger? logger) {
      _logger = logger;
   }


   public void SetSize(int width, int height) {
      if (_controller is not null)
         _controller.Bounds = new Rectangle(0, 0, width, height);
   }


   public async Task ExecuteScriptAsync(string javascript) {
      if (_controller is not null) {
         // this will blow up if not run on the UI thread, so the SynchronizationContext needs to have been wired up correctly
         await _controller.CoreWebView2.ExecuteScriptAsync(javascript);
      }
   }


   private void initController(HWND hwnd) {
      // Start initializing WebView2 in a fire-and-forget manner. Errors will be handled in the initialization function
      _ = initControllerAsync(hwnd, handleMessageFromWebView, _logger,
                              controller => _controller = controller);
   }


   private void handleMessageFromWebView(string webMessageReceived) {
      _logger?.LogTrace("web message received: {message}", webMessageReceived);
      MessageFromPage?.Invoke(webMessageReceived);
   }
}
