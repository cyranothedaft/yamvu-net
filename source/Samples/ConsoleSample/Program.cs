using System;

namespace ConsoleSample;

internal class Program {
   static void Main(string[] args) {
      Console.CursorVisible = false;

      // row-major character layout
      char[,] view
            // = new char[10, 10]; 
            =
               {
                  { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', },
                  { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', },
                  { '2', '3', '4', '5', '6', '7', '8', '9', '0', '1', },
                  { '3', '4', '5', '6', '7', '8', '9', '0', '1', '2', },
                  { '4', '5', '6', '7', '8', '9', '0', '1', '2', '3', },
                  { '5', '6', '7', '8', '9', '0', '1', '2', '3', '4', },
                  { '6', '7', '8', '9', '0', '1', '2', '3', '4', '5', },
                  { '7', '8', '9', '0', '1', '2', '3', '4', '5', '6', },
                  { '8', '9', '0', '1', '2', '3', '4', '5', '6', '7', },
                  { '9', '0', '1', '2', '3', '4', '5', '6', '7', '8', },
               };

      (int left, int top) normalCursorPos = initializeView(10, 10);
      writeViewWithDirectAddressing(view);

      // restore expected cursor position
      Console.SetCursorPosition(normalCursorPos.left, normalCursorPos.top);
      return;
   }


   private static (int left, int top) initializeView(int height, int width) {
      Console.WriteLine();
      Console.WriteLine("╔" + (new string('═', width)) + "╗");
      for (int i = 0; i < height; ++i)
         Console.WriteLine("║" + (new string(' ', width)) + "║");
      Console.WriteLine("╚" + (new string('═', width)) + "╝");
      Console.WriteLine();
      return Console.GetCursorPosition();
   }


   private static void writeViewWithDirectAddressing(char[,] view) {
      for (int r = 0; r <= view.GetUpperBound(0); ++r)
      for (int c = 0; c <= view.GetUpperBound(1); ++c) {
         writeAt((left: c, top: r),
                 view[r, c]);
      }
   }


   private static void writeAt((int left, int top) relativePos, char ch) {
      (int left, int top) absolutePos = relToAbs(relativePos);
      Console.SetCursorPosition(absolutePos.left,
                                absolutePos.top);
      Console.Write(ch);
   }


   private static (int left, int top) relToAbs((int left, int top) relativePos)
      // account for the blank line and our fancy border
      => (relativePos.left + 1,
          relativePos.top  + 2);
}
