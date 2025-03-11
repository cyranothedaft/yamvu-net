namespace ConsoleSample;


internal class ConsoleSurface(int Width, int Height) : IDisposable {
   private bool _savedCursorVisibility;
   private (int left, int top) _savedCursorPos;


   public void Initialize() {
      saveCursorVisibility();
      Console.CursorVisible = false;
      writeInitialView(Width, Height);
      saveCursorPosition();
   }


   public void DrawView(View view) {
      writeViewWithDirectPositioning(view.Chars);
   }


   private void saveCursorVisibility() {
      _savedCursorVisibility = Console.CursorVisible;
   }


   private void restoreCursorVisibility() {
      Console.CursorVisible = _savedCursorVisibility;
   }

   
   private void saveCursorPosition() {
      _savedCursorPos = Console.GetCursorPosition();
   }
   

   private void restoreCursorPosition() {
      Console.SetCursorPosition(_savedCursorPos.left,
                                _savedCursorPos.top);
   }


   private static void  writeInitialView(int height, int width) {
      Console.WriteLine();
      Console.WriteLine   ("╔" + (new string('═', width)) + "╗");
      for (int i = 0; i < height; ++i)
         Console.WriteLine("║" + (new string(' ', width)) + "║");
      Console.WriteLine   ("╚" + (new string('═', width)) + "╝");
      Console.WriteLine();
   }


   private static void writeViewWithDirectPositioning(char[,] view) {
      for (int r = 0; r <= view.GetUpperBound(0); ++r)
      for (int c = 0; c <= view.GetUpperBound(1); ++c) {
         writeAt((left: c, top: r),
                 view[r, c]);
      }
   }


   private static void writeAt((int left, int top) viewPos, char ch) {
      (int left, int top) surfacePos = convertViewToSurfaceCoords(viewPos);
      Console.SetCursorPosition(surfacePos.left,
                                surfacePos.top);
      Console.Write(ch);
   }


   private static (int left, int top) convertViewToSurfaceCoords((int left, int top) viewPos)
      // account for the blank line and our fancy border
      => (viewPos.left + 1,
          viewPos.top  + 2);


   public void Dispose() {
      restoreCursorPosition();
      restoreCursorVisibility();
   }
}
