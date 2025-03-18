using System;

namespace ConsoleSample.View;

internal static class ViewDisplayer {
   public static void OutputView(View view) {
      Console.WriteLine("~~~[ The View ]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      foreach (string line in view.TextLines) {
         Console.WriteLine(line);
      }

      //Console.WriteLine("/~~ The current View ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      //foreach (string line in view.TextLines) {
      //   // indent each line
      //   Console.WriteLine("|  " + line);
      //}
   }
}
