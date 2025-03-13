using System;

namespace ConsoleSample;

internal record View(
      string Caption,
      string Status,
      // row-major character layout (for example, Char[1,9] is the second row and tenth column
      char[,] Chars
) {
   public const int Width  = 12;
   public const int Height = 10;
   public const string StaticHeader = "Press Any Key  or  [Esc] to quit)";
}
