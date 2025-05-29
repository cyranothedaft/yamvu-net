using System;
using Windows.Win32;


namespace yamvu.Extensions.WebView.Library;

public static class MinimalConsole {
   public static void ShowConsole() {
      PInvoke.AllocConsole();
   }
}
