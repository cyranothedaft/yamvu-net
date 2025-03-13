using System.Diagnostics;
using ConsoleSample.AppEvents;
using ConsoleSample.UIBasics;



namespace ConsoleSample;


internal class ConsoleSurface : IDisposable {
   private readonly bool _savedCursorVisibility;
   private readonly (int left, int top) _savedCursorPos;
   private readonly KeyPressMonitor _keyPressMonitor;

   public int Width { get; }
   public int Height { get; }


   public ConsoleSurface(int width, int height) {
      Width  = width;
      Height = height;

      _savedCursorVisibility = Console.CursorVisible;
      Console.CursorVisible = false;

      writeFrame(Width, Height); // TODO: do this outside of the constructor, somewhere
      _savedCursorPos = Console.GetCursorPosition();

      _keyPressMonitor = new KeyPressMonitor();
   }


   public void DrawView(View view) {
      writeCharArrayWithDirectPositioning(view.Chars);
      writeAt(CaptionPosition, view.Caption);
      writeAt(StatusPosition,  view.Status);
   }


   public void StartMonitoringKeyboard(CancellationToken cancellationToken = default) {
      _keyPressMonitor.Start(cancellationToken);
   }


   public void StopMonitoringKeyboard() {
      _keyPressMonitor.StopSafe();
   }


   public void WireUpKeyPressEvents(AppEventSinks appEventSinks) {
      _keyPressMonitor .KeyPressed += (_, args) => {
                          Debug.WriteLine($"** keys: {args.KeyInfo.DisplayText()}"); // TODO: log to ILogger instead
                          bool isHandled = appEventSinks.HandleAppKeyPress(args.KeyInfo.ToKeyPressInfo());
                       };
   }


   private void restoreCursorVisibility() {
      Console.CursorVisible = _savedCursorVisibility;
   }


   private void restoreCursorPosition() {
      Console.SetCursorPosition(_savedCursorPos.left,
                                _savedCursorPos.top);
   }


   private (int left, int top) CaptionPosition => (this.Width + 8, 1);
   private (int left, int top) StatusPosition  => (this.Width + 8, Height);

   private static void writeFrame(int height, int width) {
      Console.WriteLine();
                                        Console.WriteLine("╔" + (new string('═', width)) + "╗ caption:");
      for (int i = 0; i < height; ++i)  Console.WriteLine("║" + (new string(' ', width)) + "║");
                                        Console.WriteLine("╚" + (new string('═', width)) + "╝ status:");
      Console.WriteLine();
   }


   private static void writeCharArrayWithDirectPositioning(char[,] view) {
      for (int r = 0; r <= view.GetUpperBound(0); ++r)
      for (int c = 0; c <= view.GetUpperBound(1); ++c) {
         writeAt((left: c, top: r),
                 view[r, c]);
      }
   }


   private static void writeAt<T>((int left, int top) viewPos, T writeable) {
      (int left, int top) surfacePos = convertViewToSurfaceCoords(viewPos);
      Console.SetCursorPosition(surfacePos.left,
                                surfacePos.top);
      Console.Write(writeable);
   }


   private static (int left, int top) convertViewToSurfaceCoords((int left, int top) viewPos)
      // account for the blank line and our fancy border
      => (viewPos.left + 1,
          viewPos.top  + 2);


   public void Dispose() {
      StopMonitoringKeyboard();
      _keyPressMonitor.Dispose();
      restoreCursorPosition();
      restoreCursorVisibility();
   }
}
