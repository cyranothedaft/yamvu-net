using System;

namespace ConsoleSample;

internal static class ViewDisplayer {
   public static void OutputView(View view) {
      Console.WriteLine("-- The latest view:");
      foreach (string line in view.TextLines) {
         // indent each line
         Console.WriteLine("   " + line);
      }
   }
}
