using System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.Extensions.Logging;


namespace yamvu.Extensions.WebView.Library.Window;

public static class MessagePump {
   public static int Run(ILogger? logger = null) {
      logger?.LogDebug("Starting message pump");
      MSG msg;
      while (PInvoke.GetMessage(out msg,
                                new HWND(), // TODO why?
                                0, 0)) {
         logger?.LogTraceWM(msg);
         PInvoke.TranslateMessage(msg);
         PInvoke.DispatchMessage(msg);
      }

      return (int)msg.wParam.Value;
   }


   private static void LogTraceWM(this ILogger logger, MSG msg) {
      logger.LogTrace("WM: {hwnd:x8} {wm:x8} {wparam:x16} {lparam:x16} ({pt_x,8:N0}, {pt_y,8:N0}) {time,8}",
                      msg.hwnd.Value, msg.message, msg.wParam.Value, msg.lParam.Value, msg.pt.x, msg.pt.y, msg.time);
   }
}
