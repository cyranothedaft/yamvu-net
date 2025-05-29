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


namespace MinimalWebViewCounterSample;


/// <summary>
/// adapted from:  https://github.com/rgwood/MinimalWebView
/// </summary>
public static class WebViewMessagePumpWindow {




   public static int Run(string windowTitle, uint backgroundColor, ILogger? logger = null) {

      Window window = Window.Create();

      window.Show();

      // Start initializing WebView2 in a fire-and-forget manner. Errors will be handled in the initialization function
      _ = initControllerAsync();

      int exitCode = runMessagePump(logger);
      return exitCode;


   }




   private static int runMessagePump(ILogger? logger) {
      logger?.LogDebug("Starting message pump");
      MSG msg;
      while (PInvoke.GetMessage(out msg,
                                new HWND(), // TODO why?
                                0, 0)) {
         PInvoke.TranslateMessage(msg);
         PInvoke.DispatchMessage(msg);
      }

      return (int)msg.wParam.Value;
   }




   private static int GetLowWord(nint value) {
      uint xy = (uint)value;
      int x = unchecked( (short)xy );
      return x;
   }


   private static int GetHighWord(nint value) {
      uint xy = (uint)value;
      int y = unchecked( (short)(xy >> 16) );
      return y;
   }



}
